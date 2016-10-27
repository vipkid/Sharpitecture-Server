using Sharpitecture.Networking;
using System.Text;
using System;

namespace Sharpitecture.Levels.IO.NBT
{
    public class NbtShort : NbtField
    {
        public override int Size
        {
            get
            {
                return 5 + Name.Length;
            }
        }

        public override byte TypeID
        {
            get
            {
                return 2;
            }
        }

        public override byte[] Serialize()
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
