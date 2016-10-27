using Sharpitecture.Networking;
using System.Text;
using System;

namespace Sharpitecture.Levels.IO.NBT
{
    public class NbtLong : NbtField
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
                return 4;
            }
        }

        public override byte[] Serialize()
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
