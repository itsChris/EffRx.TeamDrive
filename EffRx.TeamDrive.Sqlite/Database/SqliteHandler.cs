using EffRx.TeamDrive.Common.Entities;
using System.Collections.Generic;
using System.Data.SQLite;

namespace EffRx.TeamDrive.Sqlite.Database
{
    public class SqliteHandler
    {
        private string databasePath;

        public string DatabasePath
        {
            get { return databasePath; }
            set { databasePath = value; }
        }

        public SqliteHandler(string dbPath)
        {
            databasePath = dbPath;
            string cs = $@"URI=file:{dbPath}";

            using (SQLiteConnection con = new SQLiteConnection(cs))
            {
                con.Open();
                string stm = "SELECT ID, OriginalName, SpaceRoot FROM tbl_space";
                using (SQLiteCommand cmd = new SQLiteCommand(stm, con))
                {
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            //System.Console.WriteLine($"ID: {rdr.GetInt32(0)} OriginalName: {rdr.GetString(1)} SpaceRoot: {rdr.GetString(2)}");
                        }
                    }
                }
            }
        }

        public List<Space> GetSpaces()
        {
            List<Space> spaceList = new List<Space>();
            string cs = $@"URI=file:{databasePath}";

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
                            //System.Console.WriteLine($"ID: {rdr.GetInt32(0)} OriginalName: {rdr.GetString(1)} SpaceRoot: {rdr.GetString(2)}");
                        }
                    }
                }
            }
            return spaceList;
        }
    }
}
