using EffRx.TeamDrive.Common.Logging;
using Microsoft.Win32;
using System;
using System.IO;
using System.Reflection;

namespace EffRx.TeamDrive.ShellExHandler
{
    internal class Program
    {
        public const string UriScheme = "EffRx-TeamDrive";

        private static SimpleLogger logger { get; set; }
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
        static void Main(string[] args)
        {
            // initiate logger
            logger = new SimpleLogger(LogFilePath);

            // dump command line args
            DumpCommandLineArguments(args);

            // it's a user registry key but requires UAC elevated permission in order to add the key .. 
            ChangeOutlookSecurity(UriScheme);

            logger.Info($"Argument count: {args.Length}");

            if (args.Length == 0)
            {
                logger.Error("ERROR: No argument provided!");
            }
            else
            {
                string path = args[0];
                if (File.Exists(path))
                {
                    logger.Info($"Good - File exists: {path}");
                    AddShellEx(path);
                }
                else
                {
                    logger.Error($"File/Path: {path} is invalid/does not exist!");
                }
            }

            logger.Error("Press enter to exit..");
            Console.Read();
        }

        private static void AddShellEx(string applicationPath)
        {
            try
            {
                logger.Info($"AddShellEx: Variable applicationPath: {applicationPath}");
                logger.Info($"AddShellEx: Will try to add registry keys and values...");
                var reg = Registry.LocalMachine.OpenSubKey("Software", true).OpenSubKey("Classes", true).CreateSubKey("AllFilesystemObjects", true).CreateSubKey("shell", true);
                RegistryKey key = reg.CreateSubKey(@"EffRx-TeamDriveShellEx\command", true);
                key.SetValue("", "\"" + applicationPath + "\" \"%1\"");
                logger.Info($"AddShellEx: Done adding registry keys and values");

            }
            catch (Exception ex)
            {
                logger.Error($"Exception: {ex.Message}");
            }
        }

        private static void ChangeOutlookSecurity(string protocolName)
        {
            try
            {
                logger.Info($"Will try to update Outlook security settings for: {protocolName} ");
                logger.Info(@"Key: HKEY_CURRENT_USER\SOFTWARE\Policies\Microsoft\Office\16.0\Common\Security\Trusted Protocols\All Applications\EffRx-TeamDrive:");
                // https://docs.microsoft.com/en-us/office365/troubleshoot/administration/enable-disable-hyperlink-warning
                //HKEY_CURRENT_USER\SOFTWARE\Policies\Microsoft\Office\16.0\Common\Security\Trusted Protocols\All Applications\EffRx-TeamDrive:

                var regKeyCurrUser = Registry.CurrentUser
                    .CreateSubKey("Software", true)
                    .CreateSubKey("Policies", true)
                    .CreateSubKey("Microsoft", true)
                    .CreateSubKey("Office", true)
                    .CreateSubKey("16.0", true)
                    .CreateSubKey("Common", true)
                    .CreateSubKey("Security", true)
                    .CreateSubKey("Trusted Protocols", true)
                    .CreateSubKey("All Applications", true);
                RegistryKey key = regKeyCurrUser.CreateSubKey(protocolName);
                logger.Info("done adding registry key");
            }
            catch (Exception ex)
            {
                logger.Error("Exception: " + ex.Message);
            }
        }
        private static void DumpCommandLineArguments(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                logger.Info($"Argument: {i} has value: {args[i]}");
            }
        }
    }
}
