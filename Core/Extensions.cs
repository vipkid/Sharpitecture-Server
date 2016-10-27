using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;

namespace Sharpitecture
{
    public static class Extensions
    {
        /// <summary>
        /// Whether two strings are similar regardless of their case
        /// </summary>
        public static bool CaselessEquals(this string a, string b)
        {
            return a.Equals(b, System.StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Compresses an array of bytes
        /// </summary>
        public static byte[] GZip(this byte[] b)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream gs = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    gs.Write(b, 0, b.Length);
                }

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Converts a string to a generic type
        /// Based off: http://stackoverflow.com/questions/2961656/generic-tryparse
        /// </summary>
        public static bool Convert<T, T1>(this T1 input, out T value)
        {
            value = default(T);
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter != null)
            {
                try
                {
                    value = (T)converter.ConvertFromString(input.ToString());
                }
                catch
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Gets a Unix Timestamp
        /// </summary>
        public static long GetUnixTimestamp()
        {
            return (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds);
        }
    }
}
