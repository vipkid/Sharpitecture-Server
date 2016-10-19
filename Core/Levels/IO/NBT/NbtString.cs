using Sharpitecture.Networking;
using System.Text;
using System;

namespace Sharpitecture.Levels.IO.NBT
{
    public class NbtString : INbtField
    {
        public string Name
        {
            get; set;
        }

        public int Size
        {
            get
            {
                return 5 + Name.Length + ((string)Value).Length;
            }
        }

        public byte TypeID
        {
            get
            {
                return 8;
            }
        }

        public object Value { get; set; }

        public byte[] Serialize()
        {
            string val = (string)Value;
            ByteBuffer buffer = new ByteBuffer(Size);
            buffer.WriteByte(TypeID);
            buffer.WriteShort((short)Name.Length);
            buffer.WriteString(Name, Encoding.ASCII, Name.Length);
            buffer.WriteShort((short)val.Length);
            buffer.WriteString(val, Encoding.ASCII, val.Length);
            return buffer.Data;
        }
    }
}
