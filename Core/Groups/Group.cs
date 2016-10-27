using Sharpitecture.Chatting;
using Sharpitecture.Utils.Config;
using Sharpitecture.Utils.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sharpitecture.Groups
{
    public class Group
    {
        /// <summary>
        /// The name of the group
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The prefix of the group
        /// </summary>
        public string Prefix { get; set; }
        
        /// <summary>
        /// The default colour of the group
        /// </summary>
        public string DefaultColour { get; set; }

        /// <summary>
        /// The file path consisting the list of members within the group
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// The list of members within this group
        /// </summary>
        public GroupList GroupList { get; private set; }

        /// <summary>
        /// The permission level of this group
        /// </summary>
        public int PermissionLevel { get; set; }

        #region Default Groups
        public static readonly Group Player = new Group("players.txt")
        {
            Name = "Player",
            PermissionLevel = 0,
            DefaultColour = c.White
        };

        public static readonly Group VIP = new Group("vips.txt")
        {
            Name = "VIP",
            PermissionLevel = 50,
            DefaultColour = c.Silver,
            Prefix = "[VIP]",
        };

        public static readonly Group Op = new Group("operators.txt")
        {
            Name = "OP",
            PermissionLevel = 100,
            Prefix = "[OP]",
            DefaultColour = c.Red,
        };
        #endregion

        public Group(string filePath)
        {
            Name = "Unnamed";
            PermissionLevel = 0;
            Prefix = string.Empty;
            DefaultColour = c.White;
            FilePath = filePath;
            GroupList = new GroupList(filePath);
        }

        /// <summary>
        /// Initialises a list of groups
        /// </summary>
        public static void Initialise()
        {
            Server.Ranks = new List<Group>();

            if (Server.UseSimpleGroups)
            {
                Server.Ranks.Add(Player);
                Server.Ranks.Add(VIP);
                Server.Ranks.Add(Op);

                Server.Ranks.ForEach(rank => rank.GroupList.Load());
            }
            else
            {
                using (StreamReader sr = new StreamReader("properties/rank.properties"))
                {
                    Group group = null;
                    while (!sr.EndOfStream)
                    {
                        string key, value, line;

                        if (!ConfigFile.ParseLine(line = sr.ReadLine(), out key, out value))
                        {
                            if (!string.IsNullOrEmpty(line) && !line.StartsWith("#"))
                                Logger.LogF("(properties/rank.properties) Hit error at line '{0}'", LogType.Error, line);
                            continue;
                        }

                        switch (key.ToLower())
                        {
                            case "rankname":
                                if (group != null) Server.Ranks.Add(group);
                                group = new Group("") { Name = value };
                                break;
                            case "color":
                            case "colour":
                                if (group == null) break;
                                if (value.StartsWith("&")) { group.DefaultColour = value; break; }
                                char tmp;
                                if (!value.Convert(out tmp)) break;
                                group.DefaultColour = "&" + tmp;
                                break;
                            case "prefix":
                                if (group == null) break;
                                group.Prefix = value;
                                break;
                            case "filename":
                                if (group == null) break;
                                group.FilePath = value;
                                group.GroupList.Path = group.FilePath;
                                break;
                            case "permission":
                                if (group == null) break;
                                int perm;
                                if (!value.Convert(out perm)) break;
                                group.PermissionLevel = perm;
                                break;
                        }
                    }
                }

                if (!Server.Ranks.Any(grp => grp.PermissionLevel == 0))
                    Server.Ranks.Add(Player);
                if (!Server.Ranks.Any(grp => grp.PermissionLevel == 50))
                    Server.Ranks.Add(VIP);
                if (!Server.Ranks.Any(grp => grp.PermissionLevel == 100))
                    Server.Ranks.Add(Op);
            }

            Server.DefaultRank = Find(Config.DefaultRank) ?? Server.Ranks.FirstOrDefault(grp => grp.PermissionLevel == 0);
        }
        
        /// <summary>
        /// Saves the groups configuration
        /// </summary>
        public static void SaveRankConfiguration()
        {
            Server.Ranks.ForEach(rank => rank.GroupList.Save());
            if (Server.UseSimpleGroups) return;

            using (StreamWriter sw = new StreamWriter("properties/rank.properties"))
            {
                Server.Ranks.ForEach(rank =>
                {
                    sw.WriteLine("RankName = " + rank.Name);
                    sw.WriteLine("Permission = " + rank.PermissionLevel);
                    sw.WriteLine("Colour = " + rank.DefaultColour);
                    sw.WriteLine("Prefix = " + rank.Prefix);
                    sw.WriteLine("FileName = " + rank.FilePath);
                    sw.WriteLine();
                });

                sw.Flush();
                sw.Close();
            }
        }

        /// <summary>
        /// Finds the group of a specified player
        /// </summary>
        public static Group FindPlayerRank(string name) => 
            Server.Ranks.FirstOrDefault(grp => grp.GroupList.Contains(name))
            ?? Server.DefaultRank;

        /// <summary>
        /// Finds a group with the specified name
        /// </summary>
        public static Group Find(string name) =>
            Server.Ranks.FirstOrDefault(grp => grp.Name.CaselessEquals(name));
    }
}
