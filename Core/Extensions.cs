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
    }
}
