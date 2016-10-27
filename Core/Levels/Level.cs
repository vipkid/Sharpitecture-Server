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
        public byte[] Blocks { get; internal set; }
        public byte[] UUID = new byte[16];
        public byte[][] PhyBlocks { get; internal set; }

        public string Authors { get; internal set; }
        public string SeedName { get; internal set; }

        public long TimeCreated { get; internal set; }
        public long LastAccessed { get; internal set; }
        public long LastModified { get; internal set; }

        /// <summary>
        /// The spawn position of the level
        /// </summary>
        public Vector3S Spawn;

        /// <summary>
        /// The spawn rotation of the level
        /// </summary>
        public Vector3B SpawnRot;

        private string _loadingTitle;
        private string _loadingCaption;

        /// <summary>
        /// The X size of the level
        /// </summary>
        public ushort Width { get; private set; }

        /// <summary>
        /// The Z size of the level
        /// </summary>
        public ushort Depth { get; private set; }

        /// <summary>
        /// The Y size of the level
        /// </summary>
        public ushort Height { get; private set; }

        /// <summary>
        /// The name of the level
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The block definition configuration for this level
        /// </summary>
        public BlockDefinitions BlockDefinitions { get; private set; }

        /// <summary>
        /// The NPCs currently on the level
        /// </summary>
        public List<Entity> NPCs { get; private set; }

        /// <summary>
        /// The players currently on the level
        /// </summary>
        public List<Player> Players
        {
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

        /// <summary>
        /// The entities currently on the level
        /// </summary>
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

        /// <summary>
        /// The MOTD of the level
        /// </summary>
        public string MOTD { get; private set; }

        public Level()
        {
            TimeCreated = Extensions.GetUnixTimestamp();
            UUID = Guid.NewGuid().ToByteArray();
        }

        public Level(string name, string seed = "")
        {
            Name = name;
            Width = 16;
            Height = 16;
            Depth = 16;
            Blocks = new byte[4096];
            Initialize(seed);
        }

        public Level(string name, ushort X, ushort Y, ushort Z, string seed = "")
        {
            if (X < 16) X = 16;
            if (Y < 16) Y = 16;
            if (Z < 16) Z = 16;
            Name = name;
            Width = X;
            Height = Y;
            Depth = Z;
            Blocks = new byte[X * Y * Z];
            Initialize(seed);
            Authors = string.Empty;

            Spawn = new Vector3S((short)(Width / 2), (short)(Height / 2 + 1), (short)(Depth / 2));
        }

        public void Initialize(string seed)
        {
            TimeCreated = Extensions.GetUnixTimestamp();
            UUID = Guid.NewGuid().ToByteArray();

            BlockDefinitions = new BlockDefinitions();
            NPCs = new List<Entity>();

            Seed.Seeds[0].Generate(this, CoreBlock.Grass, CoreBlock.Dirt);
            SeedName = seed;
        }

        /// <summary>
        /// Generates a compatible block data array for network use
        /// </summary>
        public byte[] Serialize(bool customBlocks, bool blockDefinitions)
        {
            if (customBlocks && blockDefinitions)
                return (byte[])Blocks.Clone();

            ByteBuffer buffer = new ByteBuffer(4 + Blocks.Length);
            buffer.WriteInt(Blocks.Length);
            for (int i = 0; i < Blocks.Length; i++)
                buffer.WriteByte(BlockDefinitions.GetFallback(Blocks[i], customBlocks, blockDefinitions));

            return buffer.Data;
        }

        /// <summary>
        /// Returns an EntityID which is currently not occupied
        /// </summary>
        public byte GetFreeEntityID()
        {
            for (int i = 0; i < 254; i++)
                if (!Entities.Any(e => e.EntityID == i))
                    return (byte)i;
            return 255;
        }

        /// <summary>
        /// Finds an entity by their EntityID
        /// </summary>
        public Entity GetEntityByID(byte id) => Entities.FirstOrDefault(e => e.EntityID == id);

        /// <summary>
        /// Sets the level spawn
        /// </summary>
        public void SetSpawn(short X, short Y, short Z, byte RotX = 0, byte RotY = 0)
        {
            Spawn.X = X;
            Spawn.Y = Y;
            Spawn.Z = Z;
            SpawnRot.X = RotX;
            SpawnRot.Y = RotY;
        }

        /// <summary>
        /// Handles a manual change sent by a player
        /// </summary>
        public void HandleManualChange(Player sender, short x, short y, short z, byte tile, bool created = true)
        {
            byte finalBlock = created ? tile : CoreBlock.Air;

            SendBlockchange(x, y, z, finalBlock);
            SetTile(x, y, z, finalBlock);
        }

        /// <summary>
        /// Sends a blockchange to all players on the level
        /// </summary>
        public void SendBlockchange(short x, short y, short z, byte tile)
        {
            Players.ForEach(player =>
            {
                player.SendBlockchange(x, y, z, tile);
            });
        }

        /// <summary>
        /// Converts a position to an index
        /// </summary>
        public int PosToInt(short x, short y, short z) => x + Width * (z + Depth * y);
        public int PosToInt(Vector3S pos) => PosToInt(pos.X, pos.Y, pos.Z);

        /// <summary>
        /// Returns whether a position is inside the level boundaries
        /// </summary>
        public bool InRange(short x, short y, short z) => (x >= 0 && x < Width) && (y >= 0 && y < Height) && (z >= 0 && z < Depth);
        public bool InRange(Vector3S pos) => InRange(pos.X, pos.Y, pos.Z);

        /// <summary>
        /// Gets the tile occupying the specified position
        /// </summary>
        public byte GetTile(short x, short y, short z)
        {
            if (!InRange(x, y, z)) return CoreBlock.Air;
            return Blocks[PosToInt(x, y, z)];
        }
        public byte GetTile(Vector3S pos) => GetTile(pos.X, pos.Y, pos.Z);

        /// <summary>
        /// Sets the tile at the specified location
        /// </summary>
        public void SetTile(short x, short y, short z, byte tile)
        {
            if (!InRange(x, y, z)) return;
            Blocks[PosToInt(x, y, z)] = tile;
        }

        /// <summary>
        /// Finds a level by their exact name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Level FindExact(string name)
        {
            return Server.Levels.FirstOrDefault(level => level.Name.CaselessEquals(name));
        }
    }
}
