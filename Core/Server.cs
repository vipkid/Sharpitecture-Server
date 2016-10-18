using Sharpitecture.Chatting;
using Sharpitecture.Database;
using Sharpitecture.Entities;
using Sharpitecture.Levels;
using Sharpitecture.Networking;
using Sharpitecture.Tasks;
using Sharpitecture.Utils.Concurrent;
using Sharpitecture.Utils.Logging;
using System.Text;

namespace Sharpitecture
{
    public static class Server
    {
        private static TcpIPListener _listener { get; set; }
        public static readonly Encoding CP437 = Encoding.GetEncoding(437);
        internal static SqlDatabase PlayerDB;
        public static VolatileList<Player> Players { get; internal set; }
        private static Scheduler _scheduler;
        public static Level MainLevel { get; private set; }
        public static VolatileList<Level> Levels { get; internal set; }

        public static void Start()
        {
            _listener = new TcpIPListener(25564);
            _listener.OnSocketConnect += ProcessConnection;
            _listener.Start();

            _scheduler = new Scheduler("Main.Scheduler");
            _scheduler.Start();

            Players = new VolatileList<Player>();
            Levels = new VolatileList<Level>();

            Config.Initialise();

            PlayerDB = new SqlDatabase("PlayerDB");

            MainLevel = new Level("main", 64, 64, 64);
            Levels.Add(MainLevel);
            EntityHandler.Initialise();
        }

        public static void QueueTask(Task task)
        {
            _scheduler.Enqueue(task);
        }

        static void ProcessConnection(SocketConnectEventArgs e)
        {
            string ip = e.Socket.RemoteEndPoint.ToString().Split(':')[0];
            Logger.LogF("{0} connected to the server", LogType.Info, ip);
            Player player = new Player(new Connection(e.Socket));
        }
    }
}
