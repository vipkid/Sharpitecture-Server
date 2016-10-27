using Sharpitecture.Networking;
using System.Text;
using System;

namespace Sharpitecture.Levels.IO.NBT
{
    public class NbtDouble : NbtField
    {
        public override int Size
        {
            get
            {
                return 11 + Name.Length;
            }
        }

        public override byte TypeID
        {
            get
            {
                return 6;
            }
        }

        public override byte[] Serialize()
        {
            ByteBuffer buffer = new ByteBuffer(Size);
            buffer.WriteByte(TypeID);
            buffer.WriteShort((short)Name.Length);
            buffer.WriteString(Name, Encoding.ASCII, Name.Length);
            buffer.WriteDouble((double)Value);
            return buffer.Data;
        }
    }
}
