using Sharpitecture.Utils.Config;
using Sharpitecture.Utils.Logging;
using System.IO;

namespace Sharpitecture
{
    public static class Starter
    {
        /// <summary>
        /// Whether the server starts with a graphical user interface
        /// </summary>
        public static bool GuiMode { get; set; }

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

                GuiMode = false;
                StartServer();
                return;
            }

            using (StreamReader reader = new StreamReader(File.OpenRead("launcher.properties")))
            {
                string line, key, value;
                while (!reader.EndOfStream)
                {
                    if (ConfigFile.ParseLine(line = reader.ReadLine(), out key, out value))
                    {
                        if (key.CaselessEquals("gui-mode"))
                        {
                            try
                            {
                                GuiMode = bool.Parse(value);
                            }
                            catch
                            {
                                GuiMode = false;
                            }
                        }
                    }
                }

                reader.Close();
            }

            StartServer();
        }

        /// <summary>
        /// Starts the server
        /// </summary>
        static void StartServer()
        {
            Logger.Initalise();
            Server.Start();
        }
    }
}
