using Sharpitecture.Chatting;
using Sharpitecture.Utils.Concurrent;
using Sharpitecture.Utils.Logging;
using System;
using System.IO;

namespace Sharpitecture.Groups
{
    public class Group
    {
        public string Name { get; set; }
        public string Prefix { get; set; }
        public string DefaultColor { get; set; }
        public string FilePath { get; set; }
        private GroupList GroupList { get; set; }
        public int PermissionLevel { get; set; }

        #region Default Groups
        public static readonly Group Player = new Group()
        {
            Name = "Player",
            PermissionLevel = 0,
            FilePath = "players.txt"
        };

        public static readonly Group VIP = new Group()
        {
            Name = "VIP",
            PermissionLevel = 50,
            DefaultColor = c.Silver,
            Prefix = "[VIP]",
            FilePath = "vips.txt"
        };

        public static readonly Group Op = new Group()
        {
            Name = "OP",
            PermissionLevel = 100,
            Prefix = "[OP]",
            DefaultColor = c.Red,
            FilePath = "operators.txt"
        };
        #endregion

        public Group()
        {
            Name = "Unnamed";
            PermissionLevel = 0;
            Prefix = string.Empty;
            DefaultColor = c.White;
            GroupList = new GroupList();
        }

        public static void Initialise()
        {
            Server.Ranks = new VolatileList<Group>();

            if (Server.UseSimpleGroups)
            {
                Server.Ranks.Add(Player);
                Server.Ranks.Add(VIP);
                Server.Ranks.Add(Op);

                Server.Ranks.ForEach(rank => rank.GroupList.Load(rank.FilePath));
            }
            else
            {
                using (StreamReader sr = new StreamReader("properties/rank.properties"))
                {
                    Group group = null;
                    while (!sr.EndOfStream)
                    {
                        string key, value, line;

                        if (!PropertyFile.Parse(line = sr.ReadLine(), out key, out value))
                        {
                            if (!string.IsNullOrEmpty(line) && !line.StartsWith("#"))
                                Logger.LogF("(properties/rank.properties) Hit error at line '{0}'", LogType.Error, line);
                            continue;
                        }

                        switch (key.ToLower())
                        {
                            case "rankname":
                                if (group != null) Server.Ranks.Add(group);
                                group = new Group() { Name = value };
                                break;
                            case "color":
                            case "colour":
                                if (group == null) break;
                                if (value.StartsWith("&")) { group.DefaultColor = value; break; }
                                char tmp;
                                if (!value.Convert(out tmp)) break;
                                group.DefaultColor = "&" + tmp;
                                break;
                            case "prefix":
                                if (group == null) break;
                                group.Prefix = value;
                                break;
                            case "filename":
                                if (group == null) break;
                                group.FilePath = value;
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

            Server.DefaultRank = Find(Config.Get<string>(Config.DefaultRank)) ?? Server.Ranks.FirstOrDefault(grp => grp.PermissionLevel == 0);
        }
        
        public static void SaveRankConfiguration()
        {
            Server.Ranks.ForEach(rank => rank.GroupList.Save(rank.FilePath));
            if (Server.UseSimpleGroups) return;

            using (StreamWriter sw = new StreamWriter("properties/rank.properties"))
            {
                Server.Ranks.ForEach(rank =>
                {
                    sw.WriteLine("RankName = " + rank.Name);
                    sw.WriteLine("Permission = " + rank.PermissionLevel);
                    sw.WriteLine("Colour = " + rank.DefaultColor);
                    sw.WriteLine("Prefix = " + rank.Prefix);
                    sw.WriteLine("FileName = " + rank.FilePath);
                    sw.WriteLine();
                });

                sw.Flush();
                sw.Close();
            }
        }

        public static Group FindPlayerRank(string name) => 
            Server.Ranks.FirstOrDefault(grp => grp.GroupList.Contains(name))
            ?? Server.DefaultRank;

        public static Group Find(string name) =>
            Server.Ranks.FirstOrDefault(grp => grp.Name.CaselessEquals(name));
    }
}
