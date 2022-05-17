using System;
using System.Windows.Forms;

namespace EffRx.TeamDrive.ShellEx
{
    internal class Program
    {
        public const string UriScheme = "EffRx-TeamDrive";

        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No command line arguments provided");
            }
            else
            {
                string arg = args[0];
                string ret = $"{UriScheme}://{arg}";
                Console.WriteLine($"Command line arguments provided: {args[0]}");
                Console.WriteLine($"Will add the following to the clipboard {ret}");
                Clipboard.SetText(ret);
            }

            Console.WriteLine("Hit enter to quit..");
            Console.Read();
        }

    }
}
