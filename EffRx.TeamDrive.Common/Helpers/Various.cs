using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace EffRx.TeamDrive.Common.Helpers
{
    public class Various
    {

        [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void SHChangeNotify(uint wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);

        public static void RefreshExplorer()
        {
            SHChangeNotify(0x08000000, 0x00000000, IntPtr.Zero, IntPtr.Zero);
        }
        public static bool IsWindowsEleven
        {
            get
            {
                return Environment.OSVersion.Version.Build >= 22000;
            }
        }

        public static void RestartExplorer()
        {
            Process p = new Process();
            foreach (Process exe in Process.GetProcesses())
            {
                if (exe.ProcessName == "explorer")
                    exe.Kill();
            }
            Process.Start("explorer.exe");
        }
    }
}
