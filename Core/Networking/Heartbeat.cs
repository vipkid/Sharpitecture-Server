using Sharpitecture.Utils.Logging;
using System;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Text;

namespace Sharpitecture.Networking
{
    public static class Heartbeat
    {
        /// <summary>
        /// The website of the heartbeat
        /// </summary>
        public const string Website
            = "http://www.classicube.net/heartbeat.jsp";

        /// <summary>
        /// The timeout of the heartbeat in milliseconds
        /// </summary>
        public const int Timeout = 15000;

        /// <summary>
        /// The URL returned by the heartbeat
        /// </summary>
        public static string HeartbeatUrl { get; private set; }
        
        private static HttpWebRequest _webRequest;

        /// <summary>
        /// Sends a heartbeat request to the website
        /// </summary>
        public static void Beat()
        {
            IPAddress webIP = null;

            foreach (IPAddress entry in Dns.GetHostAddresses("www.classicube.net"))
            {
                if (entry.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    webIP = entry;
                }
            }

            _webRequest = (HttpWebRequest)WebRequest.Create("http://" + webIP + ":80/heartbeat.jsp");
            
            byte[] parameters = GetParameters();

            _webRequest.Method = "POST";
            _webRequest.ContentType = "application/x-www-form-urlencoded";
            _webRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
            _webRequest.Timeout = 30000;
            _webRequest.ContentLength = parameters.Length;
            _webRequest.Host = "www.classicube.net";

            using (Stream writer = _webRequest.GetRequestStream())
            {
                writer.Write(parameters, 0, parameters.Length);
            }

            using (StreamReader reader = new StreamReader(_webRequest.GetResponse().GetResponseStream()))
            {
                string url = reader.ReadToEnd();
                Logger.Log("URL Found: " + url, LogType.Debug);
            }
        }

        /// <summary>
        /// Gets the heartbeat parameters
        /// </summary>
        static byte[] GetParameters()
        {
            string parameters = string.Format(
                "&name={0}&port={1}&max={2}&public={3}&version=7&salt={4}&users={5}&software=Sharpitecture",
                Uri.EscapeDataString(Config.Name),
                Config.Port,
                Config.MaxPlayers,
                Config.IsPublic,
                Server.Salt,
                Server.Players.Count
                );
            

            return Encoding.UTF8.GetBytes(parameters);
        }
    }
}
