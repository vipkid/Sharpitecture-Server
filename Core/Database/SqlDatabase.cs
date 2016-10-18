using Sharpitecture.Utils.Logging;
using System;
using System.Data.SQLite;
using System.IO;

namespace Sharpitecture.Database
{
    public class SqlDatabase
    {
        private readonly string _name;

        public string Name { get { return _name; } }
        public string ConnString { get { return "Data Source=" + _name + ".db;"; } }

        public SqlDatabase(string name)
        {
            _name = name;
            if (!Directory.Exists("database")) Directory.CreateDirectory("database");

            if (string.IsNullOrEmpty(_name) || _name.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
                throw new FormatException("Database name is null or contains illegal characters");

            if (!File.Exists(string.Format("database/{0}.db", _name)))
                File.Create(string.Format("database/{0}.db", _name)).Dispose();
        }

        public void ExecuteQuery(string command)
        {
            SQLiteConnection conn = new SQLiteConnection(ConnString);
            SQLiteCommand cmd = new SQLiteCommand(command, conn);
            conn.Open();

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (SQLiteException ex)
            {
                Logger.LogF("[SQL] Error: {0}", LogType.Error, ex.Message);
            }

            conn.Close();
            conn.Dispose();
        }
    }
}
