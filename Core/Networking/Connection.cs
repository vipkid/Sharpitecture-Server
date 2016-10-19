using System;
using System.Net.Sockets;

namespace Sharpitecture.Networking
{
    public class Connection
    {
        #region API
        public delegate void DataReadEvent(ByteBuffer buffer);

        public event DataReadEvent OnDataRead = null;
        #endregion

        private Socket _socket;
        private ByteBuffer _buffer;
        private byte[] _tmpBuffer = new byte[4192];
        private readonly string _ipAddress;

        private bool _errorOccurred = false;
        private int _errorCode = 0;

        public string IPAddress { get { return _ipAddress; } }
        public bool ErrorOccurred { get { return _errorOccurred; } }

        public Connection(Socket sock)
        {
            _socket = sock;
            _ipAddress = sock.RemoteEndPoint.ToString().Split(':')[0];
            _socket.BeginReceive(_tmpBuffer, 0, _tmpBuffer.Length, SocketFlags.None, ReadCallback, null);
            _buffer = new ByteBuffer(4192);
        }

        public void ReadCallback(IAsyncResult result)
        {
            int length = _socket.EndReceive(result);
            _buffer.Write(_tmpBuffer, 0, length);

            if (OnDataRead == null)
                throw new NotImplementedException();

            OnDataRead(_buffer);
            _socket.BeginReceive(_tmpBuffer, 0, _tmpBuffer.Length, SocketFlags.None, ReadCallback, null);
        }

        public void SendRaw(byte[] data)
        {
            try
            {
                _socket.BeginSend(data, 0, data.Length, SocketFlags.None, null, null);
            }
            catch (SocketException ex)
            {
                _errorOccurred = true;
                _errorCode = ex.ErrorCode;
            }
        }
    }
}
