using Sharpitecture.Utils.Concurrent;
using System;
using System.Threading;

namespace Sharpitecture.Utils.Logging
{
    public static class Logger
    {
        private static VolatileQueue<Log> PendingLogs = new VolatileQueue<Log>();
        private static bool _initalised = false;
        private static Thread _logThread;

        public static void Log(string message, LogType type)
        {
            Log log = new Log(message, type, DateTime.Now);
            PendingLogs.Enqueue(log);
        }

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

        public static void Initalise()
        {
            if (_initalised) return;
            _logThread = new Thread(MainLoop) { Name = "Sharpitecture.Logger", IsBackground = true };
            _logThread.Start();
            Log("Logger successfully initialised", LogType.Debug);
        }

        static void MainLoop()
        {
            int retries;

            while (true)
            {
                retries = 0;
                Log log;
                while (!PendingLogs.IsEmpty)
                {
                    if (!PendingLogs.TryDequeue(out log))
                        retries++;
                    else
                    {
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

                    if (retries > 5)
                    {
                        Console.WriteLine("Failed to append log.");
                        break;
                    }
                }
                Thread.Sleep(10);
            }
        }
    }
}
