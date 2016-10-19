using Sharpitecture.Networking;
using System.Text;
using System;

namespace Sharpitecture.Levels.IO.NBT
{
    public class NbtFloat : INbtField
    {
        public string Name
        {
            get; set;
        }

        public int Size
        {
            get
            {
                return 7 + Name.Length;
            }
        }

        public byte TypeID
        {
            get
            {
                return 5;
            }
        }

        public object Value { get; set; }

        public byte[] Serialize()
        {
            ByteBuffer buffer = new ByteBuffer(Size);
            buffer.WriteByte(TypeID);
            buffer.WriteShort((short)Name.Length);
            buffer.WriteString(Name, Encoding.ASCII, Name.Length);
            buffer.WriteFloat((float)Value);
            return buffer.Data;
        }
    }
}
