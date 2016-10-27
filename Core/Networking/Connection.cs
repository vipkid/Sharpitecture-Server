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

        /// <summary>
        /// The socket bound to this connection object
        /// </summary>
        public Socket Socket { get; private set; }

        /// <summary>
        /// Whether a socket error has occurred
        /// </summary>
        public bool ErrorOccurred { get; private set; }

        /// <summary>
        /// The socket error code of the most recent error
        /// </summary>
        public int ErrorCode { get; private set; }

        /// <summary>
        /// The IP Address of the current connection
        /// </summary>
        public string IPAddress { get; private set; }

        private ByteBuffer _buffer;
        private byte[] _tmpBuffer = new byte[4192];

        public Connection(Socket sock)
        {
            Socket = sock;
            IPAddress = sock.RemoteEndPoint.ToString().Split(':')[0];
            Socket.BeginReceive(_tmpBuffer, 0, _tmpBuffer.Length, SocketFlags.None, ReadCallback, null);
            _buffer = new ByteBuffer(4192);
        }

        /// <summary>
        /// Reads incoming messages
        /// </summary>
        void ReadCallback(IAsyncResult result)
        {
            int length = Socket.EndReceive(result);
            _buffer.Write(_tmpBuffer, 0, length);

            if (OnDataRead == null)
                throw new NotImplementedException();

            OnDataRead(_buffer);
            Socket.BeginReceive(_tmpBuffer, 0, _tmpBuffer.Length, SocketFlags.None, ReadCallback, null);
        }

        /// <summary>
        /// Sends a raw message to the client
        /// </summary>
        public void SendRaw(byte[] data)
        {
            try
            {
                Socket.BeginSend(data, 0, data.Length, SocketFlags.None, null, null);
            }
            catch (SocketException ex)
            {
                ErrorOccurred = true;
                ErrorCode = ex.ErrorCode;
            }
        }
    }
}
