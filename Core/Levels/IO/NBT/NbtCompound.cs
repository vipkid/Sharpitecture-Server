using Sharpitecture.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpitecture.Levels.IO.NBT
{
    public class NbtCompound : INbtField
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

        public byte TypeID
        {
            get
            {
                return 10;
            }
        }

        public string Name { get; set; }
        private List<INbtField> _fields = new List<INbtField>();
        public object Value { get; set; }

        public int Size
        {
            get
            {
                return _fields.Sum(f => f.Size) + 4 + Name.Length;
            }
        }

        public byte[] Serialize()
        {
            ByteBuffer buffer = new ByteBuffer(Size);
            buffer.WriteByte(TypeID);
            buffer.WriteShort((short)Name.Length);
            buffer.WriteString(Name, Encoding.ASCII, Name.Length);

            foreach (INbtField field in _fields)
                buffer.Write(field.Serialize(), 0, field.Size);

            buffer.WriteByte(0);
            return buffer.Data;
        }

        public void AddField(INbtField field) => _fields.Add(field);

        public INbtField GetField(string fieldName) => _fields.FirstOrDefault(field => field.Name.CaselessEquals(fieldName));
    }
}
