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
        [Config(CfgCategories.Server, "name", "Sharpitecture Server", 
            "The name of the server")]
        public static string Name { get; set; }

        [Config(CfgCategories.Server, "motd", "This server is under development", 
            "The MOTD displayed on the client loading screen")]
        public static string MOTD { get; set; }
        
        [Config(CfgCategories.Server, "default-rank", "guest",
            "The default rank assigned to a player when they connect")]
        public static string DefaultRank { get; set; }

        [Config(CfgCategories.Server, "main-level", "main",
            "The default level a player joins when they connect")]
        public static string MainLevel { get; set; }

        [Config(CfgCategories.Server, "port", 25565,
            "The port used by the server")]
        public static int Port { get; set; }

        [Config(CfgCategories.Server, "max-players", 20,
            "The maximum number of players allowed to join the server")]
        public static int MaxPlayers { get; set; }

        [Config(CfgCategories.Server, "public", true,
            "Whether the server should show up on the server list")]
        public static bool IsPublic { get; set; }

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
                case "port": Port = int.Parse(value); break;
                case "public": IsPublic = bool.Parse(value); break;
                case "max-players": MaxPlayers = int.Parse(value); break;
            }
        }
    }

    public static class CfgCategories
    {
        public const string Server = "Server";
    }
}
