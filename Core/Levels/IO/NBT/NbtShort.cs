using Sharpitecture.Networking;
using System.Text;
using System;

namespace Sharpitecture.Levels.IO.NBT
{
    public class NbtShort : INbtField
    {
        public string Name
        {
            get; set;
        }

        public int Size
        {
            get
            {
                return 5 + Name.Length;
            }
        }

        public byte TypeID
        {
            get
            {
                return 2;
            }
        }

        public object Value { get; set; }

        public byte[] Serialize()
        {
            ByteBuffer buffer = new ByteBuffer(Size);
            buffer.WriteByte(TypeID);
            buffer.WriteShort((short)Name.Length);
            buffer.WriteString(Name, Encoding.ASCII, Name.Length);
            buffer.WriteShort(short.Parse(Value.ToString()));
            return buffer.Data;
        }
    }
}
