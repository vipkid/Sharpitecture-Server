using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Sharpitecture
{
    public static class Config
    {
        private static Dictionary<string, string> _configurations;

        public const string Name = "server-name",
                            MOTD = "server-motd";

        public static void Initialise()
        {
            _configurations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            AddConfig<string>(Name, "Sharpitecture Server");
            AddConfig<string>(MOTD, "Work in progress....");
        }

        public static T GetConfig<T>(string configName)
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
        
        public static bool AddConfig<T>(string configName, string value = null)
        {
            if (TypeDescriptor.GetConverter(typeof(T)) == null)
                return false;

            _configurations.Add(configName, value);
            return true;
        }
    }
}
