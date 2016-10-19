using Sharpitecture.Networking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharpitecture.Levels.IO.NBT
{
    public class NbtCompound : INbtField
    {
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
    }
}
