using System;
using System.Collections.Generic;
using System.Threading;

namespace Sharpitecture.Utils.Logging
{
    public static class Logger
    {
        /// <summary>
        /// The pending logs waiting to be written
        /// </summary>
        public static Queue<Log> PendingLogs { get; set; }

        /// <summary>
        /// Whether the logger has started
        /// </summary>
        public static bool Initalised { get; private set; }

        /// <summary>
        /// The thread handling the logs
        /// </summary>
        public static Thread LogThread { get; set; }

        private static object lockObject = new object();

        /// <summary>
        /// Logs a regular message
        /// </summary>
        public static void Log(string message, LogType type)
        {
            Log log = new Log(message, type, DateTime.Now);
            PendingLogs.Enqueue(log);
        }

        /// <summary>
        /// Logs a formatted message
        /// </summary>
        public static void LogF(string message, LogType type, params object[] args)
        {
            try
            {
                string msg = string.Format(message, args);
                Log(msg, type);
            }
            catch (FormatException)
            {
                Log(message, type);
            }
        }

        /// <summary>
        /// Initialises the logger
        /// </summary>
        public static void Initalise()
        {
            if (Initalised) return;
            PendingLogs = new Queue<Log>();
            LogThread = new Thread(MainLoop) { Name = "Sharpitecture.Logger", IsBackground = true };
            LogThread.Start();
            Log("Logger successfully initialised", LogType.Debug);
        }

        static void MainLoop()
        {

            while (true)
            {
                Log log;
                while (PendingLogs.Count > 0)
                {
                    lock (lockObject)
                    {
                        log = PendingLogs.Dequeue();
                        if (!Starter.GuiMode)
                        {
                            if (log.type == LogType.Debug)
                            {
#if DEBUG
                                Console.WriteLine("[{0}] [{1}] {2}", log.type.ToString().PadRight(7), log.time.ToString("HH:mm:ss"), log.data);
                                break;
#endif
                            }
                            else
                                Console.WriteLine("[{0}] [{1}] {2}", log.type.ToString().PadRight(7), log.time.ToString("HH:mm:ss"), log.data);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                }

                Thread.Sleep(10);
            }
        }
    }
}
