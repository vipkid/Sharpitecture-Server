using Sharpitecture.Networking;
using System.Text;
using System;

namespace Sharpitecture.Levels.IO.NBT
{
    public class NbtLong : INbtField
    {

        public byte[] ByteArrayValue
        {
            get
            {
                byte[] val;
                if (!Value.Convert(out val))
                    throw new FormatException();
                return val;
            }
        }

        public byte ByteValue
        {
            get
            {
                byte val;
                if (!Value.Convert(out val))
                    throw new FormatException();
                return val;
            }
        }

        public double DoubleValue
        {
            get
            {
                double val;
                if (!Value.Convert(out val))
                    throw new FormatException();
                return val;
            }
        }

        public float FloatValue
        {
            get
            {
                float val;
                if (!Value.Convert(out val))
                    throw new FormatException();
                return val;
            }
        }

        public int IntValue
        {
            get
            {
                int val;
                if (!Value.Convert(out val))
                    throw new FormatException();
                return val;
            }
        }

        public long LongValue
        {
            get
            {
                long val;
                if (!Value.Convert(out val))
                    throw new FormatException();
                return val;
            }
        }

        public short ShortValue
        {
            get
            {
                short val;
                if (!Value.Convert(out val))
                    throw new FormatException();
                return val;
            }
        }

        public string StringValue
        {
            get
            {
                return Value.ToString();
            }
        }

        public string Name
        {
            get; set;
        }

        public int Size
        {
            get
            {
                return 11 + Name.Length;
            }
        }

        public byte TypeID
        {
            get
            {
                return 4;
            }
        }

        public object Value { get; set; }

        public byte[] Serialize()
        {
            ByteBuffer buffer = new ByteBuffer(Size);
            buffer.WriteByte(TypeID);
            buffer.WriteShort((short)Name.Length);
            buffer.WriteString(Name, Encoding.ASCII, Name.Length);
            buffer.WriteLong((long)Value);
            return buffer.Data;
        }
    }
}
