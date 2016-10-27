using Sharpitecture.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sharpitecture.API.Commands
{
    public class Command
    {
        /// <summary>
        /// name of the command
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// List of shortcuts of the command
        /// </summary>
        public string[] Shortcuts { get; internal set; }

        /// <summary>
        /// Permission level required to execute command
        /// </summary>
        public int Permission { get; internal set; }

        /// <summary>
        /// Whether command can be used
        /// </summary>
        public bool Enabled { get; internal set; }

        /// <summary>
        /// Whether command can be used via Console
        /// </summary>
        public bool ConsoleUsable { get; internal set; }

        /// <summary>
        /// Whether command can be used via IRC
        /// </summary>
        public bool IRCUsable { get; internal set; }

        /// <summary>
        /// Command handler executed by player
        /// </summary>
        public Action<Player, string> Handler;

        /// <summary>
        /// The list of commands supported by the server
        /// </summary>
        public static List<Command> Commands { get; private set; }

        public static void Initialise()
        {
            Commands = new List<Command>();

            Type[] types = Assembly.GetExecutingAssembly().GetTypes();

            foreach (Type type in types)
            {
                if (type.IsAbstract && type.IsSealed)
                {
                    foreach (FieldInfo field in type.GetFields())
                    {
                        if (field.FieldType == typeof(Command))
                        {
                            var comm = Activator.CreateInstance(typeof(Command));
                            Command cmd = (Command)field.GetValue(comm);
                            Commands.Add(cmd);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Finds a command with the given name
        /// </summary>
        public static Command Find(string cmd)
        { 
            return Commands.FirstOrDefault(cd => cd.Name.CaselessEquals(cmd) || cd.Shortcuts.Any(sh => sh.CaselessEquals(cmd)));
        }

        /// <summary>
        /// Checks whether a player is found
        /// </summary>
        public static bool CheckIfPlayerExists(Player player, string target, out Player found)
        {
            found = Player.Find(target);

            if (found == null)
            {
                player.SendMessage("Could not find specified player.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Whether the player can execute an action on the target
        /// </summary>
        public static bool EqualOrGreaterPerms(Player player, Player target)
        {
            if (player.Group.PermissionLevel <= target.Group.PermissionLevel)
            {
                player.SendMessage("Cannot perform action on player with greater or equal rank.");
                return false;
            }

            return true;
        }
    }
}
