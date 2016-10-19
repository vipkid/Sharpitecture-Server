using Sharpitecture.Groups;
using Sharpitecture.Levels;
using Sharpitecture.Maths;
using System.Collections.Generic;

namespace Sharpitecture.Entities
{
    public abstract partial class Entity
    {
        protected Vector3S _position;
        protected Vector3S _oldPosition;
        protected Vector3S _posChange;
        protected Vector3B _binaryRotation;
        protected Vector3B _oldRotation;
        protected Vector3B _rotChange;
        private string _name;
        private string _hoverName;
        protected Level _level;
        private List<byte> _VisibleIDs;
        protected byte _entityID;
        public Group Group { get; set; }

        /// <summary>
        /// The name of the entity
        /// </summary>
        public virtual string Name { get { return _name; } set { _name = value; } }

        /// <summary>
        /// The hover name of the entity
        /// </summary>
        public virtual string HoverName { get { return _hoverName; } set { _hoverName = value; } }

        /// <summary>
        /// The chat name of the entity
        /// </summary>
        public virtual string ChatName { get { return ChatColour + _name; } }

        /// <summary>
        /// The chat colour of the entity
        /// </summary>
        public virtual string ChatColour { get; set; } = "&f";

        /// <summary>
        /// The rotation of the entity in degrees
        /// </summary>
        public Vector3F NormalRotation { get { return _binaryRotation.Scale(360F / 256F); } }

        /// <summary>
        /// The rotation of the entity in binary degrees
        /// </summary>
        public Vector3B Rotation { get { return _binaryRotation; } }

        /// <summary>
        /// The current level the entity is on
        /// </summary>
        public virtual Level Level { get { return _level; } }

        /// <summary>
        /// The ID bound to the entity given to other clients
        /// </summary>
        public byte EntityID { get { return _entityID; } }

        /// <summary>
        /// The list of visible Entity IDs to the entity
        /// </summary>
        public List<byte> VisibleIDs { get { return _VisibleIDs; } }

        /// <summary>
        /// The position of the entity
        /// </summary>
        public virtual Vector3S Position { get { return _position; } set { _position = value; } }

        /// <summary>
        /// The block position of the entity
        /// </summary>
        public virtual Vector3F BlockPosition { get { return _position.Scale(1 / 32f); } }


        public Vector3S PositionChange { get { return _posChange; } }
        public Vector3B RotationChange { get { return _rotChange; } }

        public Entity()
        {
            _VisibleIDs = new List<byte>();
        }

        public virtual void SetPosition(short X, short Y, short Z)
        {
            _position.X = X;
            _position.Y = Y;
            _position.Z = Z;
        }

        public virtual void SendToMap(Level level)
        {
            EntityHandler.DespawnAllEntities(this, true);
            _level = level;
            _entityID = level.GetFreeEntityID();
            EntityHandler.SpawnAllEntities(this, _level, true);
        }
    }
}
