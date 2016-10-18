using Sharpitecture.Utils.Logging;
using System;
using System.Net;
using System.Text;

namespace Sharpitecture.Networking
{
    public class ByteBuffer
    {
        private byte[] _buffer;
        private readonly int _maxLength;
        private int _index;

        public int Length { get { return _maxLength; } }
        public int Position { get { return _index; } }
        public byte[] Data { get { return _buffer; } }

        public ByteBuffer(int maxLength)
        {
            _maxLength = maxLength;
            _buffer = new byte[_maxLength];
            _index = 0;
        }

        public void Write(byte[] data, int start = 0, int count = 0)
        {
            if (count == 0) count = data.Length;
            int endwrite = _index + count;

            if (endwrite > _buffer.Length)
            {
                Logger.Log("Buffer overflow.", LogType.Error);
                throw new IndexOutOfRangeException();
            }

            /*for (int i = _index, _aindex = start; i < endwrite; i++)
                _buffer[i] = data[_aindex++];*/

            Buffer.BlockCopy(data, start, _buffer, _index, count);

            _index = endwrite;
        }

        public void WriteByte(byte b)
        {
            //if (_buffer.Length < _index)
            _buffer[_index++] = b;
        }

        public void WriteShort(short val)
        {
            if (BitConverter.IsLittleEndian)
                val = IPAddress.HostToNetworkOrder(val);
            Write(BitConverter.GetBytes(val), 0, 2);
        }

        public void WriteInt(int val)
        {
            if (BitConverter.IsLittleEndian)
                val = IPAddress.HostToNetworkOrder(val);
            Write(BitConverter.GetBytes(val), 0, 4);
        }

        public void WriteString(string text, Encoding encoding)
        {
            text = text.PadRight(64);
            if (text.Length > 64) text = text.Substring(0, 64);

            Write(encoding.GetBytes(text), 0, 64);
        }

        public void WriteBoolean(bool val) => WriteByte((byte)(val ? 1 : 0));

        public bool ReadBoolean() => ReadByte() != 0;

        public byte ReadByte()
        {
            byte value = _buffer[0];
            _index--;
            Buffer.BlockCopy(_buffer, 1, _buffer, 0, _index);
            return value;
        }

        public short ReadShort()
        {
            short value = (short)((_buffer[0] << 8) | _buffer[1]);
            _index -= 2;
            Buffer.BlockCopy(_buffer, 2, _buffer, 0, _index);
            return value;
        }

        public string ReadString(Encoding encoding, bool trimWhitespace = true)
        {
            string value = encoding.GetString(_buffer, 0, 64);
            if (trimWhitespace) value = value.Trim();
            _index -= 64;
            Buffer.BlockCopy(_buffer, 64, _buffer, 0, _index);
            return value;
        }

        public void SetPosition(int pos)
        {
            if (pos >= _buffer.Length)
                throw new IndexOutOfRangeException();
            _index = pos;
        }
    }
}
