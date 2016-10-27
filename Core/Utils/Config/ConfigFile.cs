using Sharpitecture.Utils.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Sharpitecture.Utils.Config
{
    public static class ConfigFile
    {
        public delegate void LineProcessor(string key, string value);

        /// <summary>
        /// Loads a configuration file
        /// </summary>
        public static bool LoadConfigFile(string path, LineProcessor processor)
        {
            if (!File.Exists(path))
            {
                FileInfo info = new FileInfo(path);
                if (!info.Directory.Exists)
                    info.Directory.Create();

                info.Create().Dispose();
                return false;
            }

            using (StreamReader reader = new StreamReader(File.OpenRead(path)))
            {
                string key, value;
                while (!reader.EndOfStream)
                {
                    if (ParseLine(reader, out key, out value))
                    {
                        processor(key, value);
                    }
                }

                reader.Close();
            }

            return true;
        }

        /// <summary>
        /// Parses a line in a config file
        /// </summary>
        static bool ParseLine(StreamReader reader, out string key, out string value)
        {
            return ParseLine(reader.ReadLine(), out key, out value);
        }

        /// <summary>
        /// Parses a line in a config file
        /// </summary>
        public static bool ParseLine(string line, out string key, out string value)
        {
            key = value = null;

            if (string.IsNullOrEmpty(line.Trim()))
                return false;

            if (line.StartsWith("#")) return false;

            if (line.IndexOf('=') == -1)
            {
                Logger.LogF("Could not parse line '{0}'", LogType.Error, line);
                return false;
            }

            int index = line.IndexOf('=');
            key = line.Substring(0, index);
            if (line.EndsWith("=")) value = string.Empty;
            else value = line.Substring(index + 1);

            key = key.Trim();
            value = value.Trim();
            return true;
        }

        /// <summary>
        /// Loads the default configurations for the given type
        /// </summary>
        public static void LoadDefaultConfigs(Type type)
        {
            PropertyInfo[] fields = type.GetProperties(BindingFlags.Static | BindingFlags.Public);

            foreach (PropertyInfo info in fields)
            {
                object[] attributes = info.GetCustomAttributes(typeof(ConfigAttribute), false);
                if (attributes.Length > 0)
                {
                    info.SetValue(null, ((ConfigAttribute)attributes[0]).DefaultValue, null);
                }
            }
        }

        /// <summary>
        /// Saves the configurations for the given type
        /// </summary>
        public static void SaveConfigFile(string path, Type type)
        {
            if (!File.Exists(path))
            {
                FileInfo info = new FileInfo(path);
                if (!info.Directory.Exists)
                    info.Directory.Create();

                info.Create().Dispose();
            }

            using (StreamWriter writer = new StreamWriter(File.OpenWrite(path)))
            {
                ConfigAttribute attrib;
                List<string> toWrite = new List<string>();
                foreach (PropertyInfo info in type.GetProperties(BindingFlags.Static | BindingFlags.Public))
                {
                    object[] attributes = info.GetCustomAttributes(typeof(ConfigAttribute), false);
                    if (attributes.Length > 0)
                    {
                        attrib = (ConfigAttribute)attributes[0];
                        writer.WriteLine("# " + attrib.Key + " - " + attrib.Descriptor + " - Example: " + attrib.DefaultValue);
                        toWrite.Add(attrib.Key + " = " + info.GetValue(null, null));
                    }
                }

                writer.WriteLine();
                toWrite.ForEach(item => writer.WriteLine(item));

                writer.Flush();
                writer.Close();
            }
        }
    }
}
