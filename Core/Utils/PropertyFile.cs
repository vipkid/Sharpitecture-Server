using Sharpitecture.Utils.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sharpitecture
{
    /// <summary>
    /// Consists of methods which simplify parsing basic property files
    /// </summary>
    public class PropertyFile
    {
        /// <summary>
        /// The list of properties stored within the properties file
        /// </summary>
        private Dictionary<string, string> Properties;

        /// <summary>
        /// The path of the properties file
        /// </summary>
        private readonly string Path;

        /// <summary>
        /// Creates an object which reads and stores the contents of a properties file
        /// </summary>
        public PropertyFile(string path)
        {
            if (!File.Exists(path)) File.Create(path).Dispose();

            Path = path;
        }

        /// <summary>
        /// Loads the contents of the properties file
        /// </summary>
        public void LoadProperties(Dictionary<string, string> _configs = null)
        {
            Properties = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            using (StreamReader reader = new StreamReader(Path))
            {
                string line = string.Empty, key = string.Empty, value = string.Empty;

                while (!reader.EndOfStream)
                {
                    line = reader.ReadLine();

                    if (!Parse(line, out key, out value))
                    {
                        Logger.LogF("Hit error at line: {0}", LogType.Error, line);
                        continue;
                    }

                    Properties.Add(key, value);
                }
                reader.Close();
            }

            if (_configs != null)
                foreach (string property in Properties.Keys)
                    _configs.Add(property, Properties[property]);
        }

        /// <summary>
        /// Gets a property and returns whether it was successful or not
        /// </summary>
        public T GetProperty<T>(string name, T valueIfFail, bool addIfFail = false)
        {
            T toReturn;

            if (!Properties.ContainsKey(name))
            {
                if (addIfFail)
                    Properties.Add(name, valueIfFail.ToString());
                return valueIfFail;
            }

            if (!Properties[name].Convert(out toReturn))
                return valueIfFail;

            return toReturn;
        }

        /// <summary>
        /// Sets a property and adds it if it doesn't exist
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetProperty(string name, string value)
        {
            if (!Properties.ContainsKey(name))
            {
                Properties.Add(name, value);
                Logger.LogF("Added property '{0}' with value '{1}", LogType.Warning, name, value);
                return;
            }

            Properties[name] = value;
        }

        /// <summary>
        /// Parses line within in property file (or text).
        /// </summary>
        public static bool Parse(string line, out string key, out string value)
        {
            string[] Parameters = line.Split(new[] { '=' }, 2);

            if (Parameters.Length != 2)
            {
                key = value = null;
                return false;
            }

            key = Parameters[0].Trim();
            value = Parameters[1].Trim();
            return true;
        }

        /// <summary>
        /// Saves a list of properties
        /// </summary>
        public void Save()
        {
            using (StreamWriter writer = new StreamWriter(File.Create(Path)))
            {
                foreach (string key in Properties.Keys)
                {
                    writer.WriteLine("{0} = {1}", key, Properties[key]);
                }

                writer.Flush();
                writer.Close();
            }
        }
    }
}
