using Sharpitecture.Utils.Logging;
using System;
using System.Data.SQLite;
using System.IO;

namespace Sharpitecture.Database
{
    public class SqlDatabase
    {
        /// <summary>
        /// The name of the database
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The connection string of the database
        /// </summary>
        public string ConnString { get { return "Data Source=" + Name + ".db;"; } }

        public SqlDatabase(string name)
        {
            Name = name;
            if (!Directory.Exists("database")) Directory.CreateDirectory("database");

            if (string.IsNullOrEmpty(Name) || Name.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
                throw new FormatException("Database name is null or contains illegal characters");

            if (!File.Exists(string.Format("database/{0}.db", Name)))
                File.Create(string.Format("database/{0}.db", Name)).Dispose();
        }

        /// <summary>
        /// Executes an SQLite command
        /// </summary>
        /// <param name="command"></param>
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
