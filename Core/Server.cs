using Sharpitecture.API.Commands;
using Sharpitecture.Chatting;
using Sharpitecture.Database;
using Sharpitecture.Entities;
using Sharpitecture.Groups;
using Sharpitecture.Levels;
using Sharpitecture.Levels.Seeds;
using Sharpitecture.Levels.IO;
using Sharpitecture.Networking;
using Sharpitecture.Tasks;
using Sharpitecture.Utils.Concurrent;
using Sharpitecture.Utils.Logging;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace Sharpitecture
{
    public static class Server
    {
        private static TcpIPListener _listener { get; set; }
        public static readonly Encoding CP437 = Encoding.GetEncoding(437);
        internal static SqlDatabase PlayerDB;
        private static Scheduler _scheduler;
        public static Level MainLevel { get; private set; }

        public static VolatileList<Level> Levels { get; internal set; }
        public static VolatileList<Group> Ranks { get; internal set; }
        public static VolatileList<Player> Players { get; internal set; }
        public static Group DefaultRank { get; internal set; }

        public static bool UseSimpleGroups = true;

        public static void Start()
        {
            _listener = new TcpIPListener(25564);
            _listener.OnSocketConnect += ProcessConnection;
            _listener.Start();

            _scheduler = new Scheduler("Main.Scheduler");
            _scheduler.Start();

            Players = new VolatileList<Player>();
            Levels = new VolatileList<Level>();

            Config.Load();

            Config.Initialise();
            Group.Initialise();
            Command.Initialise();
            Seed.Initialise();
            EntityHandler.Initialise();

            PlayerDB = new SqlDatabase("PlayerDB");

            MainLevel = new Level("main", 64, 64, 64);
            Levels.Add(MainLevel);
            NbtLoader.Save(MainLevel, false);
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
