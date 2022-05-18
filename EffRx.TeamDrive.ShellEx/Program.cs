using System;
using System.Windows.Forms;
using EffRx.TeamDrive.Sqlite.Database;

namespace EffRx.TeamDrive.ShellEx
{
    internal class Program
    {
        public const string UriScheme = "EffRx-TeamDrive";

        [STAThread]
        static void Main(string[] args)
        {
            string s = string.Empty;

            if (args.Length == 0)
            {
                Console.WriteLine("No command line arguments provided");
            }
            else
            {
                Console.WriteLine($"Command line arguments provided: {args[0]}");
                string arg = args[0];

                string searchfor = arg.Replace(@"\", @"/");

                SqliteHandler sqliteHandler = new SqliteHandler(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\TeamDrive\teamdrive.sqlite");

                var spaces = sqliteHandler.GetSpaces();

                foreach (var space in spaces)
                {
                    if (searchfor.Contains(space.SpaceRoot))
                    {
                        int pos = space.SpaceRoot.LastIndexOf("/") + 1;

                        Console.WriteLine($"Found space with ID: {space.Id} | OriginalName: {space.OriginalName}");
                        string cutted = space.SpaceRoot.Substring(pos, space.SpaceRoot.Length - pos);
                        Console.WriteLine($"cutted: {cutted}");
                        string spaceret = space.SpaceRoot.Replace(cutted, string.Empty);
                        string argesc = arg.Replace(@"\", @"/");

                        string ret = $@"{UriScheme}:""{argesc.Replace(spaceret, string.Empty)}""";
                        Console.WriteLine($"Will add the following to the clipboard {ret}");
                        Clipboard.SetText(ret);
                    }
                    else
                    {
                        Console.WriteLine($"Skipping space with ID: {space.Id} | OriginalName: {space.OriginalName} | SpaceRoot: {space.SpaceRoot}");
                    }
                }
            }

            Console.WriteLine("Hit enter to quit..");
            Console.Read();
        }

    }
}
