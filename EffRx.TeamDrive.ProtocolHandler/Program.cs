using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Reflection;
using EffRx.TeamDrive.Sqlite.Database;

namespace EffRx.TeamDrive.ProtocolHandler
{
    internal class Program
    {
        public const string UriScheme = "EffRx-TeamDrive";
        public const string BinAddShellEx = "EffRx.TeamDrive.ShellExHandler.exe";
        public const string BinProtocolHandler = "EffRx.TeamDrive.ShellEx.exe";

        static void Main(string[] args)
        {

            SqliteHandler sqliteHandler = new SqliteHandler(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\TeamDrive\teamdrive.sqlite");
            var spaces = sqliteHandler.GetSpaces();

            Console.WriteLine($"args.length: {args.Length}");
            // check command line args
            if (args.Length > 0)
            {
                switch (args[0].ToLower())
                {
                    case "register":
                        Console.WriteLine($"Will register protocol handler: {UriScheme}");
                        RegisterUrlProtocol(UriScheme, Assembly.GetExecutingAssembly().Location);
                        Console.WriteLine("done");
                        break;
                    case "addshellex":
                        Console.WriteLine($"Will try to add Shell Extension");
                        Process process = new Process();
                        process.StartInfo.FileName = BinAddShellEx;
                        process.StartInfo.Arguments = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\" + BinProtocolHandler;
                        process.Start();
                        Console.WriteLine("done");
                        break;
                    default:
                        if (args[0].ToLower().StartsWith(UriScheme.ToLower()))
                        {
                            try
                            {
                                foreach (var arg in args)
                                {
                                    
                                }

                                Console.WriteLine($"It's a Link -> {args[0]}");
                                Console.WriteLine($"Will search in database for space... ");
                                string link = args[0];
                                Console.WriteLine(link);
                                link = link.Replace(UriScheme + ":", "");
                                Console.WriteLine(link);
                                int pos = link.IndexOf('/');
                                Console.WriteLine(pos);
                                string space = link.Substring(0, pos);
                                Console.WriteLine(space);
                                foreach (var objspace in spaces)
                                {
                                    if (objspace.SpaceRoot.Contains(space))
                                    {
                                        Console.WriteLine($"Found space in database: ID {objspace.Id} | OriginalName: {objspace.OriginalName} | SpaceRoot: {objspace.SpaceRoot}");
                                        string p = objspace.SpaceRoot.Replace(space, "") + link;
                                        Process proc = new Process();
                                        proc.StartInfo.Arguments = p;
                                        proc.StartInfo.FileName = "explorer.exe";
                                        proc.Start();
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                Console.Read();
                            }
                        }
                        else
                        {
                            Console.WriteLine("EffRx-TeamDrive Protocol Handler");
                            Console.WriteLine($"Argument: {args[0]} is unknown!");
                        }
                        break;
                }
            }
            Console.WriteLine("Press enter to exit..");
            Console.Read();
        }

        public static void RegisterUrlProtocol(string protocolName, string applicationPath)
        {
            try
            {
                var regKeyCurrUser = Registry.CurrentUser.OpenSubKey("Software", true).OpenSubKey("Classes", true);
                RegistryKey key = regKeyCurrUser.CreateSubKey(protocolName);
                key.SetValue("URL Protocol", protocolName);
                key.CreateSubKey(@"shell\open\command").SetValue("", "\"" + applicationPath + "\" \"%1\"");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                throw;
            }
        }
    }
}
