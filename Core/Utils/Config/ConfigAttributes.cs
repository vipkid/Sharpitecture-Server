using System;

namespace Sharpitecture.Utils.Config
{
    public class ConfigAttribute : Attribute
    {
        /// <summary>
        /// The key of the configuration
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The default value of the configuration
        /// </summary>
        public object DefaultValue { get; set; }

        /// <summary>
        /// The category of the configuration
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// The description of the field
        /// </summary>
        public string Descriptor { get; set; }

        public ConfigAttribute(string category, string configName, object defaultValue, string descriptor)
        {
            this.Category = category;
            this.Key = configName;
            this.DefaultValue = defaultValue;
            this.Descriptor = descriptor;
        }
    }
}
