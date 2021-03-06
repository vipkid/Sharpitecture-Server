﻿using Sharpitecture.API.Commands;
using Sharpitecture.Database;
using Sharpitecture.Entities;
using Sharpitecture.Groups;
using Sharpitecture.Levels;
using Sharpitecture.Levels.Seeds;
using Sharpitecture.Levels.IO;
using Sharpitecture.Networking;
using Sharpitecture.Tasks;
using Sharpitecture.Utils.Logging;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Sharpitecture
{
    public static partial class Server
    {
        /// <summary>
        /// The TcpListener for game client connections
        /// </summary>
        public static TcpIPListener Listener { get; private set; }

        /// <summary>
        /// The object used for CP437 Encoding
        /// </summary>
        public static readonly Encoding CP437 = Encoding.GetEncoding(437);

        /// <summary>
        /// The player database
        /// </summary>
        internal static SqlDatabase PlayerDB;

        /// <summary>
        /// The main task scheduler for the server
        /// </summary>
        public static Scheduler Scheduler { get; private set; }

        /// <summary>
        /// The main level of the server
        /// </summary>
        public static Level MainLevel { get; private set; }

        /// <summary>
        /// The list of loaded levels
        /// </summary>
        public static List<Level> Levels { get; internal set; }

        /// <summary>
        /// The list of ranks
        /// </summary>
        public static List<Group> Ranks { get; internal set; }

        /// <summary>
        /// The list of connected players
        /// </summary>
        public static List<Player> Players { get; internal set; }

        /// <summary>
        /// The default rank a player is set
        /// </summary>
        public static Group DefaultRank { get; internal set; }

        /// <summary>
        /// Whether a simple group list should be used
        /// </summary>
        public static bool UseSimpleGroups = true;

        /// <summary>
        /// Base62 key required for clients to join the server
        /// </summary>
        public static string Salt { get; private set; }

        /// <summary>
        /// Boots up the server
        /// </summary>
        public static void Start()
        {
            Config.Load();

            Listener = new TcpIPListener(Config.Port);
            Listener.OnSocketConnect += ProcessConnection;
            Listener.Start();

            GenerateSalt();

            Scheduler = new Scheduler("Main.Scheduler");
            Scheduler.Start();

            Players = new List<Player>();
            Levels = new List<Level>();
            
            Group.Initialise();
            Command.Initialise();
            Seed.Initialise();
            EntityHandler.Initialise();

            PlayerDB = new SqlDatabase("PlayerDB");
            
            MainLevel = NbtLoader.Load(Config.MainLevel) ?? new Level("main", 64, 64, 64);
            Levels.Add(MainLevel);

            Heartbeat.Beat();
        }

        /// <summary>
        /// Queues a task to the scheduler
        /// </summary>
        public static void QueueTask(Task task)
        {
            Scheduler.Enqueue(task);
        }

        /// <summary>
        /// Handles an incoming connection
        /// </summary>
        static void ProcessConnection(SocketConnectEventArgs e)
        {
            string ip = e.Socket.RemoteEndPoint.ToString().Split(':')[0];
            Logger.LogF("{0} connected to the server", LogType.Info, ip);
            Player player = new Player(new Connection(e.Socket));
        }

        /// <summary>
        /// Generates the server salt
        /// </summary>
        static void GenerateSalt()
        {
            Salt = string.Empty;
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            byte[] tmp = new byte[1];

            for (int i = 0; i < 16;)
            {
                rng.GetBytes(tmp);
                if (!char.IsLetterOrDigit((char)tmp[0]))
                    continue;
                Salt += tmp[0].ToString();
                i++;
            }
        }
    }
}
