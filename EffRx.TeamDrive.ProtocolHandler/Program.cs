using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Reflection;
using EffRx.TeamDrive.Sqlite.Database;
using EffRx.TeamDrive.Common.Entities;
using EffRx.TeamDrive.Common.Logging;
using System.IO;
using EffRx.TeamDrive.Common.Helpers;

namespace EffRx.TeamDrive.ProtocolHandler
{
    internal class Program
    {
        public const string UriScheme = "EffRx-TeamDrive";
        public const string BinAddShellEx = "EffRx.TeamDrive.ShellExHandler.exe";
        public const string BinProtocolHandler = "EffRx.TeamDrive.ShellEx.exe";

        private static string TeamDriveSqliteDB
        {
            get
            {
                var path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\TeamDrive\teamdrive.sqlite";
                return (path);
            }
        }
        private static string LogFilePath
        {
            get
            {
                {
                    string datetimeFormat = DateTime.Now.ToString("yyyy-MM-dd");
                    var ret = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + $@"\{datetimeFormat}-" +
                                Assembly.GetExecutingAssembly().GetName().Name + ".log";
                    return ret;
                }
            }
        }

        private static SimpleLogger logger { get; set; }
        private static SqliteHandler sqliteHandler { get; set; }
        static void Main(string[] args)
        {
            // initiate logger
            logger = new SimpleLogger(LogFilePath);

            // initiate sqlLite..
            sqliteHandler = new SqliteHandler(TeamDriveSqliteDB, logger);
            var spaces = sqliteHandler.GetSpaces();

            logger.Info($"args.length: {args.Length}");

            // check command line args
            if (args.Length > 0)
            {
                switch (args[0].ToLower())
                {
                    case "?":
                        Console.WriteLine($"Argument can be: register | addshellex | win10menu | a valid {UriScheme} link");
                        break;
                    case "register":
                        logger.Info($"Will register protocol handler: {UriScheme}");
                        RegisterUrlProtocol(UriScheme, Assembly.GetExecutingAssembly().Location);
                        logger.Info("done");
                        break;
                    case "addshellex":
                        logger.Info($"Will try to add Shell Extension");
                        AddShellExtension();
                        logger.Info("done");
                        break;
                    case "win10menu":
                        logger.Info("Will revert to windows 10 context menu style..");
                        RevertToW10ContextMenu();
                        logger.Info("done");
                        break;
                    default:
                        if (args[0].ToLower().StartsWith(UriScheme.ToLower()))
                        {
                            try
                            {
                                ProtocolLink protocolLink = new ProtocolLink(args, UriScheme);

                                logger.Info($"It's a link -> {args[0]}");

                                foreach (var objspace in spaces)
                                {
                                    logger.Info($"Checking if space: {objspace.SpaceRoot.ToLower()} contains: {protocolLink.SpaceFromLink.ToLower()}");
                                    if (objspace.SpaceRoot.ToLower().Contains(protocolLink.SpaceFromLink.ToLower()))
                                    {
                                        logger.Info($"Found space in database: ID {objspace.Id} | OriginalName: {objspace.OriginalName} | SpaceRoot: {objspace.SpaceRoot}");
                                        string p = objspace.SpaceRoot.ToLower().Replace(protocolLink.SpaceFromLink.ToLower(), "") + protocolLink.LinkWithoutProtocolPrefix;
                                        logger.Info($"Will try to start process with argument: {p}");
                                        var ret = Process.Start("explorer.exe", p.Replace(@"/", @"\"));
                                        logger.Debug($"Process id returned: {ret.Id}");
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.Message);
                                Console.Read();
                            }
                        }
                        else
                        {
                            logger.Info("EffRx-TeamDrive Protocol Handler");
                            logger.Info($"Argument: {args[0]} is unknown!");
                        }
                        break;
                }
            }
            logger.Info("Press enter to exit..");
            Console.Read();
        }

        private static void RevertToW10ContextMenu()
        {
            if (Various.IsWindowsEleven)
            {
                logger.Info("RevertToW10ContextMenu: OS is Windows 11");
            }
            else
            {
                logger.Warning("RevertToW10ContextMenu: OS is not Windows 11.. calling this function makes no sense, since context menu ist pre-Win11 style..");
            }
            logger.Info("RevertToW10ContextMenu: Will try to modify registry...");
            try
            {
                var regKeyCurrUser = Registry.CurrentUser
                    .OpenSubKey("Software", true)
                    .OpenSubKey("Classes", true)
                    .CreateSubKey("CLSID", true);
                RegistryKey key = regKeyCurrUser
                    .CreateSubKey("{86ca1aa0-34aa-4e8b-a509-50c905bae2a2}", true)
                    .CreateSubKey("InprocServer32", true);

                logger.Info("RevertToW10ContextMenu: done!");

                logger.Info("RevertToW10ContextMenu: Will refresh explorer.. ");
                Various.RestartExplorer();
                logger.Info("RevertToW10ContextMenu: Done");
            }
            catch (Exception ex)
            {
                logger.Error($"RevertToW10ContextMenu: {ex}");
            }
        }

        private static void AddShellExtension()
        {
            string arg = '"' + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\" + BinProtocolHandler + '"';

            logger.Info($"AddShellExtension: BinaryToCall: {BinAddShellEx}");
            logger.Info($"AddShellExtension: Arguments: {arg}");

            if (File.Exists(BinAddShellEx))
            {

                Process process = new Process();
                process.StartInfo.FileName = BinAddShellEx;
                process.StartInfo.Arguments = arg;
                process.Start();
                logger.Info($"AddShellExtension: Returned ProcessId: {process.Id}");
            }
            else
            {
                logger.Error($"File does not exist: {BinAddShellEx}");
            }
        }

        public static void RegisterUrlProtocol(string protocolName, string applicationPath)
        {
            try
            {
                logger.Info($"RegisterUrlProtocol: Variable protocolName: {protocolName}");
                logger.Info($"RegisterUrlProtocol: Variable applicationPath: {applicationPath}");
                logger.Info("RegisterUrlProtocol: Will try to add registry keys and values...");
                var regKeyCurrUser = Registry.CurrentUser.OpenSubKey("Software", true).OpenSubKey("Classes", true);
                RegistryKey key = regKeyCurrUser.CreateSubKey(protocolName);
                key.SetValue("URL Protocol", protocolName);
                key.CreateSubKey(@"shell\open\command").SetValue("", "\"" + applicationPath + "\" \"%1\"");
                logger.Info("RegisterUrlProtocol: Done adding registry keys and values");
            }
            catch (Exception ex)
            {
                logger.Error("Exception: " + ex.Message);
            }
        }
    }
}
