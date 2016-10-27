using Sharpitecture.Entities;

namespace Sharpitecture.API.Commands
{
    /// <summary>
    /// A list of developer commands
    /// Use of these commands is not recommended
    /// </summary>
    public static class DevCommands
    {
        public static Command LoadPlugin = new Command()
        {
            Name = "LoadPlugin",
            ConsoleUsable = true,
            Enabled = true,
            IRCUsable = true,
            Permission = 100,
            Shortcuts = new[] { "lp" },
            Handler = LoadPluginCommand
        };

        public static void LoadPluginCommand(Player player, string parameters)
        {
            if (!Plugin.Load(parameters))
            {
                player.SendMessage("Could not find specified plugin");
                return;
            }

            player.SendMessage("&ePlugin '&c" + parameters.ToLower() + "&e' loaded!");
        }
    }
}
