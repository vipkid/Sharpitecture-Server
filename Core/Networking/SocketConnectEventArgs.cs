using System;
using System.Net.Sockets;

namespace Sharpitecture.Networking
{
    public class SocketConnectEventArgs : EventArgs
    {
        /// <summary>
        /// The target socket connecting
        /// </summary>
        public Socket Socket;

        public SocketConnectEventArgs(Socket s)
        {
            Socket = s;
        }
    }
}
