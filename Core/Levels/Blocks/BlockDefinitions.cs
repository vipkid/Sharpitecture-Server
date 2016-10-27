using Sharpitecture.Maths;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sharpitecture.Levels.Blocks
{
    public sealed class Block
    {
        public string Name { get; private set; }
        public byte ID { get; private set; }
        public byte NorthTexture { get; private set; }
        public byte EastTexture { get; private set; }
        public byte SouthTexture { get; private set; }
        public byte WestTexture { get; private set; }
        public byte TopTexture { get; private set; }
        public byte BottomTexture { get; private set; }
        public byte SpeedModifier { get; private set; }
        public byte TransmitsLight { get; private set; }
        public byte Glows { get; private set; }
        public Solidity CollisionType { get; private set; }
        public WalkSound WalkSound { get; private set; }
        public RenderType RenderType { get; private set; }
        public byte FogDensity { get; private set; }
        public byte Fallback { get; private set; }
        public Vector3B MinBB;
        public Vector3B MaxBB;
        public Vector3B FogColour;

        public static byte GetCustomBlocksFallback(byte id)
        {
            switch (id)
            {
                case 50: return 44;
                case 51: return 39;
                case 52: return 12;
                case 53: return 0;
                case 54: return 0;
                case 55: return 33;
                case 56: return 25;
                case 57: return 3;
                case 58: return 29;
                case 59: return 28;
                case 60: return 20;
                case 61: return 42;
                case 62: return 49;
                case 63: return 36;
                case 64: return 5;
                case 65: return 1;
                default: return id;
            }
        }
    }

    public sealed class BlockDefinitions
    {
        /// <summary>
        /// The list of block definitions
        /// </summary>
        private List<Block> Definitions { get; set; }

        /// <summary>
        /// Gets a block by its ID
        /// </summary>
        public Block this[int index]
        {
            get
            {
                return Definitions.FirstOrDefault(block => block.ID == index);
            }
        }

        public BlockDefinitions()
        {
            Definitions = new List<Block>();
            Definitions.Capacity = byte.MaxValue - 66;
        }

        /// <summary>
        /// Adds a block definition
        /// </summary>
        public bool AddDefinition(Level level, Block block)
        {
            if (Definitions.Any(b => block.ID == b.ID))
                return false;
            Definitions.Add(block);

            //Update the definitions to all players here
            return true;
        }

        /// <summary>
        /// Removes a block definition
        /// </summary>
        public bool RemoveDefinition(byte ID)
        {
            Block block = Definitions.FirstOrDefault(b => b.ID == ID);
            if (block == null)
                return false;
            Definitions.Remove(block);

            //Remove the definitions to all players here
            return true;
        }

        /// <summary>
        /// Returns the fallback of a specified block
        /// </summary>
        public byte GetFallback(byte id, bool customblocks, bool blockDefinitions)
        {
            if ((blockDefinitions && id > 65) || Definitions.Any(b => b.ID == id))
                return id;

            if (id <= 49 || (customblocks && id <= 65))
                return id;

            Block block = Definitions.Find(b => b.ID == id);
            if (block.Fallback > 49 && !customblocks) return Block.GetCustomBlocksFallback(block.Fallback);
            return block.Fallback;
        }
    }
}
