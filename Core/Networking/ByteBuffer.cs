using Sharpitecture.Utils.Logging;
using System;
using System.Net;
using System.Text;

namespace Sharpitecture.Networking
{
    public class ByteBuffer
    {
        private byte[] Buffer;

        /// <summary>
        /// The length of the byte buffer
        /// </summary>
        public int Length { get { return Buffer.Length; } }

        /// <summary>
        /// The current index of the byte buffer
        /// </summary>
        public int Position { get; private set; }

        /// <summary>
        /// The raw data/buffer
        /// </summary>
        public byte[] Data { get { return Buffer; } }

        public ByteBuffer(int maxLength)
        {
            Buffer = new byte[maxLength];
            Position = 0;
        }

        public ByteBuffer(Opcode opcode)
        {
            Buffer = new byte[opcode.length];
            Buffer[0] = opcode.id;
            Position = 1;
        }

        /// <summary>
        /// Writes a byte array to the byte buffer
        /// </summary>
        public void Write(byte[] data, int start = 0, int count = 0)
        {
            if (count == 0) count = data.Length;
            int endwrite = Position + count;

            if (endwrite > Buffer.Length)
            {
                Logger.Log("Buffer overflow.", LogType.Error);
                throw new IndexOutOfRangeException();
            }

            System.Buffer.BlockCopy(data, start, Buffer, Position, count);

            Position = endwrite;
        }

        /// <summary>
        /// Writes a byte value to the byte buffer
        /// </summary>
        public void WriteByte(byte b)
        {
            Buffer[Position++] = b;
        }

        /// <summary>
        /// Writes a short value to the byte buffer
        /// </summary>
        public void WriteShort(short val)
        {
            if (BitConverter.IsLittleEndian)
                val = IPAddress.HostToNetworkOrder(val);
            Write(BitConverter.GetBytes(val), 0, 2);
        }

        /// <summary>
        /// Writes an int value to the byte buffer
        /// </summary>
        /// <param name="val"></param>
        public void WriteInt(int val)
        {
            if (BitConverter.IsLittleEndian)
                val = IPAddress.HostToNetworkOrder(val);
            Write(BitConverter.GetBytes(val), 0, 4);
        }

        /// <summary>
        /// Writes a single value to the byte buffer
        /// </summary>
        public void WriteFloat(float val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            Write(bytes, 0, 4);
        }

        /// <summary>
        /// Writes a long value to the byte buffer
        /// </summary>
        public void WriteLong(long val)
        {
            if (BitConverter.IsLittleEndian)
                val = IPAddress.HostToNetworkOrder(val);
            Write(BitConverter.GetBytes(val), 0, 2);
        }

        /// <summary>
        /// Writes a double value to the byte buffer
        /// </summary>
        public void WriteDouble(double val)
        {
            byte[] bytes = BitConverter.GetBytes(val);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            Write(bytes, 0, 8);
        }

        /// <summary>
        /// Writes a string value to the byte buffer
        /// </summary>
        public void WriteString(string text, Encoding encoding, int length = 64)
        {
            text = text.PadRight(length);
            if (text.Length > length) text = text.Substring(0, length);

            Write(encoding.GetBytes(text), 0, length);
        }

        /// <summary>
        /// Writes a boolean value to the byte buffer
        /// </summary>
        public void WriteBoolean(bool val)
        {
            WriteByte((byte)(val ? 1 : 0));
        }

        /// <summary>
        /// Reads a boolean value
        /// </summary>
        public bool ReadBoolean()
        {
            return ReadByte() == 1;
        }

        /// <summary>
        /// Reads a byte value
        /// </summary>
        public byte ReadByte()
        {
            byte value = Buffer[0];
            Position--;
            System.Buffer.BlockCopy(Buffer, 1, Buffer, 0, Position);
            return value;
        }
        
        /// <summary>
        /// Reads a short value
        /// </summary>
        /// <returns></returns>
        public short ReadShort()
        {
            short value = (short)((Buffer[0] << 8) | Buffer[1]);
            Position -= 2;
            System.Buffer.BlockCopy(Buffer, 2, Buffer, 0, Position);
            return value;
        }

        /// <summary>
        /// Reads a string value
        /// </summary>
        public string ReadString(Encoding encoding, bool trimWhitespace = true)
        {
            string value = encoding.GetString(Buffer, 0, 64);
            if (trimWhitespace) value = value.Trim();
            Position -= 64;
            System.Buffer.BlockCopy(Buffer, 64, Buffer, 0, Position);
            return value;
        }

        /// <summary>
        /// Sets the position of the byte buffer
        /// </summary>
        public void SetPosition(int pos)
        {
            if (pos >= Buffer.Length)
                throw new IndexOutOfRangeException();
            Position = pos;
        }
    }
}
