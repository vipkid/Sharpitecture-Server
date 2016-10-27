using Sharpitecture.Entities;
using Sharpitecture.Groups;
using System.Collections.Generic;

namespace Sharpitecture.API.Commands
{
    /// <summary>
    /// A list of commands with give information
    /// </summary>
    public static class InfoCommands
    {
        public static Command Who = new Command()
        {
            Name = "Who",
            ConsoleUsable = true,
            Enabled = true,
            IRCUsable = true,
            Permission = -1,
            Shortcuts = new string[] { "players" },
            Handler = WhoCommand
        };

        public static void WhoCommand(Player player, string parameters)
        {
            List<Group> onlineGroups = new List<Group>();

            Server.Players.ForEach(p =>
            {
                if (!onlineGroups.Contains(p.Group))
                    onlineGroups.Add(p.Group);
            });

            player.SendMessage(string.Format("There {0} &c{1}&e player{2} online:",
                Server.Players.Count == 1 ? "is" : "are",
                Server.Players.Count,
                Server.Players.Count == 1 ? string.Empty : "s"));

            onlineGroups.ForEach(group =>
            {
                string text = group.DefaultColour + ":" + group.Name + "s: ";
                foreach (Player p in Server.Players.FindAll(p => p.Group == group))
                    text += p.Name + ", ";
                player.SendMessage(text);
            });
        }
    }
}
