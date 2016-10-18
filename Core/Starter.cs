using Sharpitecture.Utils.Logging;
using System.IO;

namespace Sharpitecture
{
    public static class Starter
    {
        private static bool _guiMode;
        //private static Server _server;

        public static bool GuiMode { get { return _guiMode; } set { _guiMode = value; } }
        //public static Server Server { get { return _server; } }

        public static void Main()
        {
            if (!File.Exists("launcher.properties"))
            {
                using (StreamWriter writer = new StreamWriter(File.Create("launcher.properties")))
                {
                    writer.WriteLine("# Below are explanations of each property field");
                    writer.WriteLine("# gui-mode - [true/false] - whether to use a GUI for the server");
                    writer.WriteLine("gui-mode=false");
                    writer.Flush();
                    writer.Close();
                }

                _guiMode = false;
                StartServer();
                return;
            }

            using (StreamReader reader = new StreamReader(File.OpenRead("launcher.properties")))
            {
                string line;
                while (!reader.EndOfStream)
                {
                    if ((line = reader.ReadLine()).StartsWith("#") || line.IndexOf('=') == -1) continue;
                    string[] parts = line.Split(new char[] { '=' }, 2);

                    string key = parts[0];
                    string value = parts[1];

                    if (key.CaselessEquals("gui-mode") && !bool.TryParse(value, out _guiMode))
                        _guiMode = false;
                }

                reader.Close();
            }

            StartServer();
        }

        private static void StartServer()
        {
            Logger.Initalise();
            Server.Start();
            //_server = new Server();
        }
    }
}
