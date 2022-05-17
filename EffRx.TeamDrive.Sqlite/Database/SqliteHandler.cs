using System.Data.SQLite;

namespace EffRx.TeamDrive.Sqlite.Database
{
    public class SqliteHandler
    {
        public SqliteHandler()
        {
            string cs = @"URI=file:C:\Solvia\EffRx\teamdrive.sqlite";

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
                            System.Console.WriteLine($"ID: {rdr.GetInt32(0)} OriginalName: {rdr.GetString(1)} SpaceRoot: {rdr.GetString(2)}");
                        }
                    }
                }
            }
        }
    }
}
