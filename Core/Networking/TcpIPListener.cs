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

        private Socket _listener;
        private readonly int _portNumber;

        public int Port { get { return _portNumber; } }

        public TcpIPListener(int portNumber)
        {
            _portNumber = portNumber;
        }

        public void Start()
        {
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, _portNumber);

            try
            {
                _listener.Bind(localEndPoint);
                _listener.Listen(int.MaxValue);
                _listener.BeginAccept(AcceptConnection, null);
                Logger.LogF("Created listening port on {0}", LogType.Info, _portNumber);
            }
            catch (SocketException)
            {
                Logger.LogF("Failed to create a listening port on '{0}'", LogType.Error, _portNumber);
                Logger.LogF("Please check if other applications are using this port.", LogType.Error, _portNumber);
            }
        }

        public void AcceptConnection(IAsyncResult result)
        {
            if (OnSocketConnect == null)
                throw new Exception("Listener on port '" + _portNumber + "' lacks a socket connect event handler.");
            OnSocketConnect(new SocketConnectEventArgs(_listener.EndAccept(result)));
            _listener.BeginAccept(AcceptConnection, null);
        }
    }
}
