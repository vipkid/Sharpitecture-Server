using Sharpitecture.Utils.Logging;
using System;
using System.Net;
using System.Net.Sockets;

namespace Sharpitecture.Networking
{
    public class TcpIPListener
    {

        #region API
        public delegate void SocketConnectEvent(SocketConnectEventArgs e);

        public event SocketConnectEvent OnSocketConnect = null;
        #endregion

        /// <summary>
        /// The socket listening for connections
        /// </summary>
        public Socket Listener { get; private set; }

        /// <summary>
        /// The port used by the listener
        /// </summary>
        public int Port { get; private set; }

        public TcpIPListener(int portNumber)
        {
            Port = portNumber;
        }

        /// <summary>
        /// Starts listening for connections
        /// </summary>
        public void Start()
        {
            Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, Port);

            try
            {
                Listener.Bind(localEndPoint);
                Listener.Listen(int.MaxValue);
                Listener.BeginAccept(AcceptConnection, null);
                Logger.LogF("Created listening port on {0}", LogType.Info, Port);
            }
            catch (SocketException)
            {
                Logger.LogF("Failed to create a listening port on '{0}'", LogType.Error, Port);
                Logger.LogF("Please check if other applications are using this port.", LogType.Error, Port);
            }
        }

        /// <summary>
        /// Accepts an incoming connection
        /// </summary>
        void AcceptConnection(IAsyncResult result)
        {
            if (OnSocketConnect == null)
                throw new Exception("Listener on port '" + Port + "' lacks a socket connect event handler.");
            OnSocketConnect(new SocketConnectEventArgs(Listener.EndAccept(result)));
            Listener.BeginAccept(AcceptConnection, null);
        }
    }
}
