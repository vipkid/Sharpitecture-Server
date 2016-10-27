using Sharpitecture.Groups;
using Sharpitecture.Levels;
using Sharpitecture.Maths;
using System.Collections.Generic;

namespace Sharpitecture.Entities
{
    public abstract partial class Entity
    {
        /// <summary>
        /// The position of the entity
        /// </summary>
        public Vector3S Position;

        /// <summary>
        /// The old position of the entity
        /// </summary>
        public Vector3S OldPosition;

        /// <summary>
        /// The change in the entity's position
        /// </summary>
        public Vector3S PositionChange;

        /// <summary>
        /// The rotation of the entity
        /// </summary>
        public Vector3B Rotation;

        /// <summary>
        /// The old rotation of the entity
        /// </summary>
        public Vector3B OldRotation;

        /// <summary>
        /// The change in the entity's rotation
        /// </summary>
        public Vector3B RotationChange;

        /// <summary>
        /// The level the entity is on
        /// </summary>
        public Level Level { get; protected set; }

        /// <summary>
        /// The list of entity IDs visible by the client
        /// </summary>
        public List<byte> VisibleIDs { get; protected set; }

        /// <summary>
        /// The entity ID of this entity
        /// </summary>
        public byte EntityID { get; protected set; }

        /// <summary>
        /// The group the entity is within
        /// </summary>
        public Group Group { get; set; }

        /// <summary>
        /// The name of the entity
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// The hover name of the entity
        /// </summary>
        public virtual string HoverName { get; set; }

        /// <summary>
        /// The chat name of the entity
        /// </summary>
        public virtual string ChatName { get { return ChatColour + Name; } }

        /// <summary>
        /// The chat colour of the entity
        /// </summary>
        public virtual string ChatColour { get; set; } = "&f";

        /// <summary>
        /// The decimal rotation of the entity
        /// </summary>
        public Vector3F NormalRotation { get { return Rotation.Scale(360F / 256F); } }

        /// <summary>
        /// The block position of the entity
        /// </summary>
        public virtual Vector3F BlockPosition { get { return Position.Scale(1 / 32f); } }

        public Entity()
        {
            VisibleIDs = new List<byte>();
        }

        /// <summary>
        /// Sets the position and the rotation of the entity
        /// </summary>
        public virtual void SetPosRot(short X, short Y, short Z, byte RotX, byte RotY)
        {
            Position.X = X;
            Position.Y = Y;
            Position.Z = Z;
            Rotation.X = RotX;
            Rotation.Y = RotY;
        }

        /// <summary>
        /// Sets the position of the entity
        /// </summary>
        public virtual void SetPosition(short X, short Y, short Z)
        {
            Position.X = X;
            Position.Y = Y;
            Position.Z = Z;
        }

        /// <summary>
        /// Sets the rotation of the entity
        /// </summary>
        public virtual void SetRotation(byte RotX, byte RotY)
        {
            Rotation.X = RotX;
            Rotation.Y = RotY;
        }

        /// <summary>
        /// Sends the entity to a different map
        /// </summary>
        public virtual void SendToMap(Level level)
        {
            EntityHandler.DespawnAllEntities(this, true);
            Level = level;
            EntityID = level.GetFreeEntityID();
            EntityHandler.SpawnAllEntities(this, Level, true);
        }
    }
}
