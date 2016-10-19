using Sharpitecture.Utils.Concurrent;
using Sharpitecture.Utils.Logging;
using System;
using System.IO;

namespace Sharpitecture.Groups
{
    public class GroupList
    {
        private VolatileList<string> items = new VolatileList<string>();
        

        /// <summary>
        /// Adds a name to the group list
        /// </summary>
        /// <param name="name"></param>
        public void Add(string name) => items.Add(name);

        /// <summary>
        /// Removes a name from the group list
        /// </summary>
        /// <param name="name"></param>
        public void Remove(string name) => items.Remove(name);

        /// <summary>
        /// Loads a group list from a given file
        /// </summary>
        /// <param name="path"></param>
        public void Load(string path)
        {
            if (!File.Exists("groups/" + path)) return;

            using (StreamReader sr = new StreamReader(File.OpenRead("groups/" + path)))
            {
                while (!sr.EndOfStream)
                    items.Add(sr.ReadLine());
                sr.Close();
            }
        }

        /// <summary>
        /// Saves a group list to the given file
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            using (StreamWriter sw = new StreamWriter(File.Create("groups/" + path)))
            {
                items.ForEach(item => sw.WriteLine(items));
                sw.Flush();
                sw.Close();
            }

            Logger.LogF("Saved group list '{0}'", LogType.Debug, path);
        }

        public bool Contains(string value) => items.Contains(value, StringComparer.OrdinalIgnoreCase); 
    }
}
