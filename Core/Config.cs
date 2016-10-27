using Sharpitecture.Groups;
using Sharpitecture.Utils.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Sharpitecture
{
    public static class Config
    {
        [Config("Server", "name", "Sharpitecture Server", 
            "The name of the server")]
        public static string Name { get; set; }

        [Config("Server", "motd", "This server is under development", 
            "The MOTD displayed on the client loading screen")]
        public static string MOTD { get; set; }
        
        [Config("Server", "default-rank", "guest",
            "The default rank assigned to a player when they connect")]
        public static string DefaultRank { get; set; }

        [Config("Server", "main-level", "main",
            "The default level a player joins when they connect")]
        public static string MainLevel { get; set; }

        /// <summary>
        /// Loads the server's core configurations
        /// </summary>
        public static void Load()
        {
            ConfigFile.LoadDefaultConfigs(typeof(Config));
            ConfigFile.LoadConfigFile("properties/server.properties", ProcessLine);
            ConfigFile.SaveConfigFile("properties/server.properties", typeof(Config));
        }

        /// <summary>
        /// Processes a line within the configuration file
        /// </summary>
        static void ProcessLine(string key, string value)
        {
            switch (key.ToLower())
            {
                case "name": Name = value; break;
                case "motd": MOTD = value; break;
                case "default-rank": DefaultRank = value; break;
                case "main-level": MainLevel = value; break;
            }
        }
    }
}
