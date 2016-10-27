using Sharpitecture.Networking;
using System.Text;

namespace Sharpitecture.Levels.IO.NBT
{
    public class NbtFloat : NbtField
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
                return 5;
            }
        }

        public override byte[] Serialize()
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
