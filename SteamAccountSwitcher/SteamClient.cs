#region

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    internal class SteamClient
    {
        public static void Launch()
        {
            Process.Start(Settings.Default.SteamPath);
        }

        public static void BigPicture()
        {
            Process.Start(Settings.Default.SteamPath, Resources.SteamBigPictureArg);
        }

        public static void LogIn(Account account)
        {
            var args = new List<string>();

            args.Add($"{Resources.SteamLoginArgument} \"{account.Username}\" \"{account.Password}\"");
            if (Settings.Default.BigPictureMode)
                args.Add(Resources.SteamBigPictureArg);

            Process.Start(Settings.Default.SteamPath, string.Join(" ", args));
        }

        public static void LogOut()
        {
            Process.Start(Settings.Default.SteamPath, Resources.SteamShutdownArgument);
        }

        public static void ForceClose()
        {
            using (var proc = Process.GetProcessesByName(Resources.Steam)[0])
                proc.CloseMainWindow();
        }

        public static string GetSteamTitle()
        {
            using (var proc = Process.GetProcessesByName(Resources.Steam)[0])
                return proc.MainWindowTitle;
        }

        public static void LogOutAuto()
        {
            if (GetSteamTitle() == Resources.SteamNotLoggedInTitle)
                ForceClose();
            else
                LogOut();
        }

        public static bool LogOutTimeout()
        {
            var timeout = 0;
            const int maxtimeout = 10000;
            const int waitstep = 100;
            if (IsSteamOpen())
            {
                LogOutAuto();
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