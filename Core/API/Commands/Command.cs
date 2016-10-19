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

        public static List<Command> Commands
        {
            get; private set;
        }

        public static void Initialise()
        {
            Commands = new List<Command>();
            //Commands.Add(DevCommands.TestCommand);

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

        public static Command Find(string cmd) 
            => Commands.FirstOrDefault(cd => cd.Name.CaselessEquals(cmd) || cd.Shortcuts.Any(sh => sh.CaselessEquals(cmd)));
    }
}
