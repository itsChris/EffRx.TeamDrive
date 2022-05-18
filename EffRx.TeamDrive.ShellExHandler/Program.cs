using Microsoft.Win32;
using System;
using System.IO;

namespace EffRx.TeamDrive.ShellExHandler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(args.Length);

            if (args.Length == 0)
            {
                Console.WriteLine("ERROR: No argument provided!");
            }
            else
            {
                string path = args[0];
                if (File.Exists(path))
                {
                    AddShellEx(path);
                }
                else
                {
                    Console.WriteLine($"File/Path: {path} is invalid/does not exist!");
                }
            }

            Console.WriteLine("Press enter to exit..");
            Console.Read();
        }

        private static void AddShellEx(string applicationPath)
        {
            try
            {
                var reg = Registry.LocalMachine.OpenSubKey("Software", true).OpenSubKey("Classes", true).OpenSubKey("AllFilesystemObjects", true).OpenSubKey("shell", true);
                RegistryKey key = reg.CreateSubKey(@"EffRx-TeamDriveShellEx\command", true);
                key.SetValue("", "\"" + applicationPath + "\" \"%1\"");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");

            }
        }
    }
}
