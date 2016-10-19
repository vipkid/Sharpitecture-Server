using System;

namespace Sharpitecture.Utils.Logging
{
    public struct Log
    {
        public string data;
        public LogType type;
        public DateTime time;

        public Log(string data, LogType type, DateTime time)
        {
            this.data = data;
            this.type = type;
            this.time = time;
        }
    }
}
