using EffRx.TeamDrive.Common.Entities;
using EffRx.TeamDrive.Common.Interfaces;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace EffRx.TeamDrive.Sqlite.Database
{
    public class SqliteHandler
    {
        private string databasePath;

        ILogger _iLogger;

        public string DatabasePath
        {
            get { return databasePath; }
            set { databasePath = value; }
        }

        public SqliteHandler(string dbPath, ILogger iLogger)
        {
            _iLogger = iLogger;
            databasePath = dbPath;

            if (!File.Exists(dbPath))
            {
                _iLogger.Error($"Database file does not exist - Database path: {dbPath}");
            }

            string cs = $@"URI=file:{dbPath}";

            _iLogger.Info($"Database path: {dbPath}");
        }

        public List<Space> GetSpaces()
        {
            List<Space> spaceList = new List<Space>();
            string cs = $@"URI=file:{databasePath}";

            try
            {
                _iLogger.Info("Trying to enumerate spaces..");
                using (SQLiteConnection conn = new SQLiteConnection(cs))
                {
                    conn.Open();
                    string stm = "SELECT ID, OriginalName, SpaceRoot FROM tbl_space";
                    using (SQLiteCommand cmd = new SQLiteCommand(stm, conn))
                    {
                        using (SQLiteDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                spaceList.Add(
                                    new Space
                                    {
                                        Id = rdr.GetInt32(0),
                                        OriginalName = rdr.GetString(1),
                                        SpaceRoot = rdr.GetString(2)
                                    });
                                _iLogger.Debug($"ID: {rdr.GetInt32(0)} OriginalName: {rdr.GetString(1)} SpaceRoot: {rdr.GetString(2)}");
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                _iLogger.Error(ex.Message);
            }
            _iLogger.Info($"Done enumerating spaces - will return {spaceList.Count} spaces");
            return spaceList;
        }
    }
}
