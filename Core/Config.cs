using Sharpitecture.Groups;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Sharpitecture
{
    public static class Config
    {
        private static Dictionary<string, string> _configurations;

        public const string Name = "server-name",
                            MOTD = "server-motd",
                            DefaultRank = "default-rank",
                            MainLevel = "main-level";

        public static void Initialise()
        {
            if (!_configurations.ContainsKey(Name)) Add<string>(Name, "Sharpitecture Server");
            if (!_configurations.ContainsKey(MOTD)) Add<string>(MOTD, "Work in progress....");
            if (!_configurations.ContainsKey(DefaultRank)) Add<string>(DefaultRank, "player");
            if (!_configurations.ContainsKey(MainLevel)) Add<string>(MainLevel, "main");
            Save();
        }

        public static T Get<T>(string configName)
        {
            T value = default(T);

            try
            {
                if (_configurations.ContainsKey(configName))
                {
                    var converter = TypeDescriptor.GetConverter(typeof(T));
                    if (converter != null)
                    {
                        return (T)converter.ConvertFromString(_configurations[configName]);
                    }
                }
            }
            catch (NotSupportedException)
            {
                return value;
            }

            return value;
        }
        
        public static bool Add<T>(string configName, string value = null)
        {
            if (TypeDescriptor.GetConverter(typeof(T)) == null)
                return false;

            _configurations.Add(configName, value);
            return true;
        }

        public static void Load()
        {
            _configurations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!Directory.Exists("properties")) { Directory.CreateDirectory("properties"); }

            PropertyFile file = new PropertyFile("properties/server.properties");
            file.LoadProperties(_configurations);
            file.Save();
        }

        public static void Save()
        {
            PropertyFile file = new PropertyFile("properties/server.properties");
            file.LoadProperties();
            foreach (string key in _configurations.Keys)
                file.SetProperty(key, _configurations[key]);
            file.Save();
        }
    }
}
