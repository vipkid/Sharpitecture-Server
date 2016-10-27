using Sharpitecture.Utils.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sharpitecture.Groups
{
    public class GroupList
    {
        /// <summary>
        /// The list of items in this group
        /// </summary>
        public List<string> Items { get; private set; } = new List<string>();

        /// <summary>
        /// The path containing the list of items in this group
        /// </summary>
        public string Path { get; internal set; }

        /// <summary>
        /// Adds a name to the group list
        /// </summary>
        public void Add(string name) => Items.Add(name);

        /// <summary>
        /// Removes a name from the group list
        /// </summary>
        public void Remove(string name) => Items.Remove(name);

        public GroupList(string path)
        {
            Path = path;
        }

        /// <summary>
        /// Loads a group list from a given file
        /// </summary>
        /// <param name="path"></param>
        public void Load()
        {
            if (!File.Exists("groups/" + Path)) return;

            using (StreamReader sr = new StreamReader(File.OpenRead("groups/" + Path)))
            {
                while (!sr.EndOfStream)
                    Items.Add(sr.ReadLine());
                sr.Close();
            }
        }

        /// <summary>
        /// Saves a group list to the given file
        /// </summary>
        /// <param name="path"></param>
        public void Save()
        {
            using (StreamWriter sw = new StreamWriter(File.Create("groups/" + Path)))
            {
                Items.ForEach(item => sw.WriteLine(item));
                sw.Flush();
                sw.Close();
            }

            Logger.LogF("Saved group list '{0}'", LogType.Debug, Path);
        }

        /// <summary>
        /// Returns whether the items contains a specified value
        /// </summary>
        public bool Contains(string value) => Items.Contains(value, StringComparer.OrdinalIgnoreCase); 
    }
}
