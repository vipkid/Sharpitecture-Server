using Sharpitecture.Maths;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sharpitecture.Levels.Blocks
{
    public sealed class Block
    {
        private string _name = "Unnamed";
        private readonly byte _id;
        private byte _nTexture;
        private byte _eTexture;
        private byte _sTexture;
        private byte _wTexture;
        private byte _bTexture;
        private byte _tTexture;
        private byte _speedModifier = 1;
        private bool _transmitsLight;
        private bool _glows;
        private Solidity _solidity = Solidity.Solid;
        private WalkSound _walkSound;
        private RenderType _renderType;
        private byte _fogDensity;
        private Vector3B _fogColour;
        private byte _fallback;

        public string Name { get { return _name; } }
        public byte ID { get { return _id; } }
        public byte Fallback { get { return _fallback; } }

        public void UpdateTexture(Level level, byte north, byte east, byte south, byte west, byte top, byte bottom)
        {
            throw new NotImplementedException();
        }

        public void SetName(Level level, string name)
        {
            throw new NotImplementedException();
        }

        public void SetBoundingBox(Level level, Vector3B min, Vector3B max)
        {
            SetBoundingBox(level, min.X, min.Y, min.Z, max.X, max.Y, max.Z);
        }

        public void SetBoundingBox(Level level, byte x1, byte y1, byte z1, byte x2, byte y2, byte z2)
        {
            throw new NotImplementedException();
        }

        public void SetSpeedModifier(float speed)
        {
            throw new NotImplementedException();
        }

        public void SetTransparent(bool transmitsLight)
        {
            throw new NotImplementedException();
        }

        public void SetRenderType(RenderType renderType)
        {
            throw new NotImplementedException();
        }

        public void SetFullBrightness(bool fullBright)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the height of the block
        /// </summary>
        [Obsolete("BlockDefinitionsExt specifies a bounding box field. Use Block.SetBoundingBox")]
        public void SetHeight(byte height)
        {
            throw new NotImplementedException();
        }

        public void SetStepSound(WalkSound sound)
        {
            throw new NotImplementedException();
        }

        public static byte GetFallback(byte id)
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
        private List<Block> _definitions;

        public BlockDefinitions()
        {
            _definitions = new List<Block>();
        }

        /// <summary>
        /// Returns the fallback of a specified block
        /// </summary>
        public byte GetFallback(byte id, bool customblocks, bool blockDefinitions)
        {
            if ((blockDefinitions && id > 65) || _definitions.Any(b => b.ID == id))
                return id;

            if (id <= 49 || (customblocks && id <= 65))
                return id;

            Block block = _definitions.Find(b => b.ID == id);
            if (block.Fallback > 49 && !customblocks) return Block.GetFallback(block.Fallback);
            return block.Fallback;
        }
    }
}
