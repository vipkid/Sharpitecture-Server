using Sharpitecture.Entities;

namespace Sharpitecture.API.Commands
{
    /// <summary>
    /// A list of moderation commands use to moderate the server
    /// </summary>
    public static class ModCommands
    {
        public static Command Kick = new Command()
        {
            Name = "kick",
            ConsoleUsable = true,
            Enabled = true,
            IRCUsable = true,
            Permission = 100,
            Shortcuts = new[] { "k" },
            Handler = KickCommand
        };

        public static void KickCommand(Player player, string parameters)
        {
            int pars = parameters.Split(' ').Length;
            string target = parameters;
            string message = "You were kicked by " + player.ChatName;

            if (pars > 1)
            {
                string[] @params = parameters.Split(new char[] { ' ' }, 2);
                target = @params[0];
                message = @params[1];
            }

            Player targetPlayer;

            if (!Command.CheckIfPlayerExists(player, target, out targetPlayer)) return;
            if (!Command.EqualOrGreaterPerms(player, targetPlayer)) return;

            targetPlayer.Kick(message);
        }
    }
}
