using Sharpitecture.Networking;
using System.Text;
using System;

namespace Sharpitecture.Levels.IO.NBT
{
    public class NbtByteArray : INbtField
    {
        public string Name
        {
            get; set;
        }

        public int Size
        {
            get
            {
                return 7 + ((byte[])Value).Length + Name.Length;
            }
        }

        public byte TypeID
        {
            get
            {
                return 7;
            }
        }

        public object Value { get; set; }

        public byte[] Serialize()
        {
            byte[] arr = ((byte[])Value);
            ByteBuffer buffer = new ByteBuffer(Size);
            buffer.WriteByte(TypeID);
            buffer.WriteShort((short)Name.Length);
            buffer.WriteString(Name, Encoding.ASCII, Name.Length);
            buffer.WriteInt(arr.Length);
            buffer.Write(arr, 0, arr.Length);
            return buffer.Data;
        }
    }
}
