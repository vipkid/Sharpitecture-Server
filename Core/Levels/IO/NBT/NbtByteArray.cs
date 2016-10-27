using Sharpitecture.Networking;
using System.Text;
using System;

namespace Sharpitecture.Levels.IO.NBT
{
    public class NbtByteArray : NbtField
    {
        public override byte[] ByteArrayValue
        {
            get
            {
                return (byte[])Value;
            }
        }

        public override byte ByteValue
        {
            get
            {
                throw new FormatException();
            }
        }

        public override double DoubleValue
        {
            get
            {
                throw new FormatException();
            }
        }

        public override float FloatValue
        {
            get
            {
                throw new FormatException();
            }
        }

        public override int IntValue
        {
            get
            {
                throw new FormatException();
            }
        }

        public override long LongValue
        {
            get
            {
                throw new FormatException();
            }
        }

        public override short ShortValue
        {
            get
            {
                throw new FormatException();
            }
        }

        public override string StringValue
        {
            get
            {
                return Value.ToString();
            }
        }

        public override int Size
        {
            get
            {
                return 7 + ByteArrayValue.Length + Name.Length;
            }
        }

        public override byte TypeID
        {
            get
            {
                return 7;
            }
        }

        public override byte[] Serialize()
        {
            byte[] arr = ByteArrayValue;
            ByteBuffer buffer = new ByteBuffer(Size);
            buffer.WriteByte(TypeID);
            buffer.WriteShort((short)Name.Length);
            buffer.WriteString(Name, Encoding.ASCII, Name.Length);
            buffer.WriteInt(arr.Length);
            buffer.Write(arr, 0, arr.Length);
            return buffer.Data;
        }
    }
}
