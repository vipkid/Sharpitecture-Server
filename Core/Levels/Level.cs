using Sharpitecture.Entities;
using Sharpitecture.Levels.Blocks;
using Sharpitecture.Levels.Seeds;
using Sharpitecture.Maths;
using Sharpitecture.Networking;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sharpitecture.Levels
{
    public sealed partial class Level
    {
        private readonly ushort _width;
        private readonly ushort _depth;
        private readonly ushort _height;
        private Vector3S _spawnPosition;
        private string _name;
        public byte[] Blocks { get; internal set; }
        public byte[] UUID = new byte[16];
        public byte[][] PhyBlocks { get; internal set; }
        private BlockDefinitions _blockDefs;

        public string Authors { get; internal set; }
        public string SeedName { get; internal set; }

        public long TimeCreated { get; internal set; }
        public long LastAccessed { get; internal set; }
        public long LastModified { get; internal set; }

        public Vector3S Spawn;
        public Vector3B SpawnRot;

        private string _loadingTitle;
        private string _loadingCaption;

        /// <summary>
        /// The X size of the level
        /// </summary>
        public ushort Width { get { return _width; } }

        /// <summary>
        /// The Z size of the level
        /// </summary>
        public ushort Depth { get { return _depth; } }

        /// <summary>
        /// The Y size of the level
        /// </summary>
        public ushort Height { get { return _height; } }

        /// <summary>
        /// The name of the level
        /// </summary>
        public string Name { get { return _name; } }

        public BlockDefinitions BlockDefinitions
        {
            get
            { return _blockDefs; }
        }

        private List<Entity> _npcs;

        public List<Entity> NPCs { get { return _npcs; } }

        public List<Player> Players {
            get
            {
                List<Player> toReturn = new List<Player>();
                Server.Players.ForEach(p =>
                {
                    if (p.Level == this)
                        toReturn.Add(p);
                });
                return toReturn;
            }
        }

        public List<Entity> Entities
        {
            get
            {
                List<Entity> toReturn = new List<Entity>();
                toReturn.AddRange(NPCs);
                toReturn.AddRange(Players.Cast<Entity>());
                return toReturn;
            }
        }

        public string LoadingScreenTitle
        {
            get
            {
                if (_loadingTitle == null)
                    _loadingTitle = Config.Get<string>(Config.Name);
                return _loadingTitle;
            }
            internal set
            {
                _loadingTitle = value;
            }
        }

        public Level()
        {
            TimeCreated = Extensions.GetUnixTimestamp();
            UUID = Guid.NewGuid().ToByteArray();
        }

        public Level(string name)
        {
            _name = name;
            _width = 16;
            _height = 16;
            _depth = 16;
            Blocks = new byte[4096];
            Initialize();
        }

        public Level(string name, short X, short Y, short Z)
        {
            if (X < 16) X = 16;
            if (Y < 16) Y = 16;
            if (Z < 16) Z = 16;
            _name = name;
            _width = (ushort)X;
            _depth = (ushort)Z;
            _height = (ushort)Y;
            Blocks = new byte[X * Y * Z];
            Initialize();
            Authors = string.Empty;

            _spawnPosition = new Vector3S((short)(_width / 2), (short)(_height / 2 + 1), (short)(_depth / 2));
        }

        public void Initialize()
        {
            TimeCreated = Extensions.GetUnixTimestamp();
            UUID = Guid.NewGuid().ToByteArray();

            _blockDefs = new BlockDefinitions();
            _npcs = new List<Entity>();

            Seed.Seeds[0].Generate(this, CoreBlock.Grass, CoreBlock.Dirt);
            SeedName = Seed.Seeds[0].Name;
        }

        public byte[] Serialize(bool customBlocks, bool blockDefinitions)
        {
            ByteBuffer buffer = new ByteBuffer(4 + Blocks.Length);
            buffer.WriteInt(Blocks.Length);
            for (int i = 0; i < Blocks.Length; i++)
                buffer.WriteByte(_blockDefs.GetFallback(Blocks[i], customBlocks, blockDefinitions));

            return buffer.Data;
        }

        public byte GetFreeEntityID()
        {
            for (int i = 0; i < 255; i++)
                if (!Entities.Any(e => e.EntityID == i))
                    return (byte)i;
            return 0;
        }

        public Entity GetEntityByID(byte id) => Entities.FirstOrDefault(e => e.EntityID == id);

        public void SetSpawn(short X, short Y, short Z)
        {
            _spawnPosition.X = X;
            _spawnPosition.Y = Y;
            _spawnPosition.Z = Z;
        }

        public void HandleManualChange(Player sender, short x, short y, short z, byte tile, bool created = true)
        {
            byte finalBlock = created ? tile : CoreBlock.Air;

            SendBlockchange(x, y, z, finalBlock);
            SetTile(x, y, z, finalBlock);
        }

        public void SendBlockchange(short x, short y, short z, byte tile)
        {
            Players.ForEach(player =>
            {
                player.SendBlockchange(x, y, z, tile);
            });
        }

        public int PosToInt(short x, short y, short z) => x + Width * (z + Depth * y);
        public int PosToInt(Vector3S pos) => PosToInt(pos.X, pos.Y, pos.Z);
        public bool InRange(short x, short y, short z) => (x >= 0 && x < Width) && (y >= 0 && y < Height) && (z >= 0 && z < Depth);
        public bool InRange(Vector3S pos) => InRange(pos.X, pos.Y, pos.Z);

        public byte GetTile(short x, short y, short z)
        {
            if (!InRange(x, y, z)) return CoreBlock.Air;
            return Blocks[PosToInt(x, y, z)];
        }
        public byte GetTile(Vector3S pos) => GetTile(pos.X, pos.Y, pos.Z);

        public void SetTile(short x, short y, short z, byte tile)
        {
            if (!InRange(x, y, z)) return;
            Blocks[PosToInt(x, y, z)] = tile;
        }

        public static Level FindExact(string name)
        {
            return Server.Levels.FirstOrDefault(level => level.Name.CaselessEquals(name));
        }
    }
}
