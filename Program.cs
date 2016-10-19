using Sharpitecture;
using System;
using System.IO;
using System.Net;

namespace SharpStarter
{
    class Program
    {
        const string UpdateURL = "http://github.com/";
        static string CurrentRevision = "";
        static bool OnlineMode = false;

        public static void Main(string[] args)
        {
            retry:

            if (!File.Exists("Core.dll"))
            {
                Console.WriteLine("Core library could not be found.\nWould you like to manually download the file?\n(Y/N): ");
                string command = "";

                while (!IsYesOrNo(command = Console.ReadLine()))
                {
                    Console.Clear();
                    Console.WriteLine("Core library could not be found.\nWould you like to manually download the file?\n(Y/N): ");
                }

                if (command.Equals("y", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Downloading the latest core library...");
                    goto retry;
                }
            }

            if (OnlineMode && CheckForLatestRevision()) DownloadLatestRevision();
            Starter.Main();
        }

        public static bool CheckForLatestRevision()
        {
            WebClient client = new WebClient();
            string revision = client.DownloadString(UpdateURL);
            return CurrentRevision != revision;
        }

        public static void DownloadLatestRevision() { }

        public static bool IsYesOrNo(string line)
        {
            return line.Equals("Y", StringComparison.OrdinalIgnoreCase) || line.Equals("N", StringComparison.OrdinalIgnoreCase);
        }
    }
}
