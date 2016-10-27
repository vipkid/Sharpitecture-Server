using Sharpitecture.Networking;
using System.Text;
using System;

namespace Sharpitecture.Levels.IO.NBT
{
    public class NbtInt : NbtField
    {
        public override int Size
        {
            get
            {
                return 7 + Name.Length;
            }
        }

        public override byte TypeID
        {
            get
            {
                return 3;
            }
        }

        public override byte[] Serialize()
        {
            ByteBuffer buffer = new ByteBuffer(Size);
            buffer.WriteByte(TypeID);
            buffer.WriteShort((short)Name.Length);
            buffer.WriteString(Name, Encoding.ASCII, Name.Length);
            buffer.WriteInt((int)Value);
            return buffer.Data;
        }
    }
}
