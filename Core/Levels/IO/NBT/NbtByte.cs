using Sharpitecture.Networking;
using System.Text;
using System;

namespace Sharpitecture.Levels.IO.NBT
{
    public class NbtByte : INbtField
    {
        public string Name
        {
            get; set;
        }

        public int Size
        {
            get
            {
                return 4 + Name.Length;
            }
        }

        public byte TypeID
        {
            get
            {
                return 1;
            }
        }

        public object Value { get; set; }

        public byte[] Serialize()
        {
            ByteBuffer buffer = new ByteBuffer(Size);
            buffer.WriteByte(TypeID);
            buffer.WriteShort((short)Name.Length);
            buffer.WriteString(Name, Encoding.ASCII, Name.Length);
            buffer.WriteByte(byte.Parse(Value.ToString()));
            return buffer.Data;
        }
    }
}
