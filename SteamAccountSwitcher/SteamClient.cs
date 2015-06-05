#region

using System.Diagnostics;
using System.IO;
using System.Threading;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    internal class SteamClient
    {
        public static void LogIn(Account account)
        {
            Process.Start(Settings.Default.SteamPath, $"-login \"{account.Username}\" \"{account.Password}\"");
        }

        public static void LogOut()
        {
            Process.Start(Settings.Default.SteamPath, "-shutdown");
        }

        public static bool LogOutAuto()
        {
            var timeout = 0;
            const int maxtimeout = 5000;
            const int waitstep = 500;
            if (IsSteamOpen())
            {
                LogOut();
                while (IsSteamOpen())
                {
                    if (timeout >= maxtimeout)
                    {
                        Popup.Show("Logout operation has timed out. Please force close steam and try again.");
                        return false;
                    }
                    Thread.Sleep(waitstep);
                    timeout += waitstep;
                }
            }
            return true;
        }

        public static string ResolvePath()
        {
            const string bit32Path = "C:\\Program Files\\Steam\\Steam.exe";
            const string bit64Path = "C:\\Program Files (x86)\\Steam\\Steam.exe";

            if (File.Exists(bit32Path))
                return bit32Path;
            if (File.Exists(bit64Path))
                return bit64Path;
            Popup.Show("Default Steam path could not be located.\r\n\r\nPlease enter Steam executable location.");
            var dia = new SteamPath();
            dia.ShowDialog();
            return dia.Path;
        }

        public static bool IsSteamOpen()
        {
            return (Process.GetProcessesByName("steam").Length > 0);
        }
    }
}