using Sharpitecture.Networking;
using System.Text;

namespace Sharpitecture.Levels.IO.NBT
{
    public class NbtByte : NbtField
    {
        public override int Size
        {
            get
            {
                return 4 + Name.Length;
            }
        }

        public override byte TypeID
        {
            get
            {
                return 1;
            }
        }

        public override byte[] Serialize()
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
