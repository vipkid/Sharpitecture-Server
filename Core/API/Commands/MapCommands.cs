using Sharpitecture.Chatting;
using Sharpitecture.Entities;
using Sharpitecture.Levels;
using Sharpitecture.Levels.IO;

namespace Sharpitecture.API.Commands
{
    /// <summary>
    /// A list of commands which can edit map properties
    /// </summary>
    public static class MapCommands
    {
        public static Command SetSpawn = new Command()
        {
            Name = "SetSpawn",
            ConsoleUsable = false,
            Enabled = true,
            Shortcuts = new string[0],
            IRCUsable = false,
            Permission = 100,
            Handler = SetSpawnHandler
        };

        public static void SetSpawnHandler(Player player, string parameters)
        {
            if (player.Level == null) { return; }

            Level level = player.Level;

            level.Spawn.X = player.Position.X;
            level.Spawn.Y = player.Position.Y;
            level.Spawn.Z = player.Position.Z;
            level.SpawnRot.X = player.Rotation.X;
            level.SpawnRot.Y = player.Rotation.Y;

            player.SendMessage("&4Updated the spawn location of the map!");
        }

        public static Command SaveLevel = new Command()
        {
            Name = "SaveLevel",
            ConsoleUsable = true,
            Enabled = true,
            Shortcuts = new string[] { "save" },
            IRCUsable = true,
            Permission = 100,
            Handler = SaveLevelHandler
        };

        public static void SaveLevelHandler(Player player, string parameters)
        {
            Level level = player.Level;

            if (!string.IsNullOrEmpty(parameters))
                level = Level.FindExact(parameters);

            if (level == null)
            {
                player.SendMessage("Could not find specified level");
                return;
            }

            NbtLoader.Save(level, true);
            Chat.MessageAll("&eLevel '&c" + level.Name + "&e' has been saved.");
        }
    }
}
