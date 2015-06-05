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
            Process.Start(Settings.Default.SteamPath,
                $"{Resources.SteamLoginArgument} \"{account.Username}\" \"{account.Password}\"");
        }

        public static void LogOut()
        {
            Process.Start(Settings.Default.SteamPath, Resources.SteamShutdownArgument);
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
            if (File.Exists(Resources.SteamPath32))
                return Resources.SteamPath32;
            if (File.Exists(Resources.SteamPath64))
                return Resources.SteamPath64;
            Popup.Show("Default Steam path could not be located.\r\n\r\nPlease enter Steam executable location.");
            var dia = new SteamPath();
            dia.ShowDialog();
            return dia.Path;
        }

        public static bool IsSteamOpen()
        {
            return (Process.GetProcessesByName(Resources.Steam).Length > 0);
        }
    }
}