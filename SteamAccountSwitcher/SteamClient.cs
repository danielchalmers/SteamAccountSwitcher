#region

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Microsoft.Win32;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    internal class SteamClient
    {
        public static void StartSteam(string args = "")
        {
            if (!ResolvePath())
            {
                Popup.Show("Steam path could not be located.");
                return;
            }
            Process.Start(Settings.Default.SteamPath, args);
        }

        public static void Launch()
        {
            StartSteam();
        }

        public static void BigPicture()
        {
            StartSteam(Resources.SteamBigPictureArg);
        }

        public static void LogIn(Account account, bool onStart = false)
        {
            var args = new List<string>();

            args.Add($"{Resources.SteamLoginArgument} \"{account.Username}\" \"{account.Password}\"");
            if (Settings.Default.BigPictureMode)
                args.Add(Resources.SteamBigPictureArg);

            if ((Settings.Default.StartSteamMinimized && Settings.Default.StartSteamMinimizedOnlyOnStartup && onStart) ||
                (Settings.Default.StartSteamMinimized && !Settings.Default.StartSteamMinimizedOnlyOnStartup))
                args.Add(Resources.SteamSilentArg);

            args.Add(Settings.Default.SteamLaunchArgs);

            StartSteam(string.Join(" ", args));
        }

        public static void LogOut()
        {
            StartSteam(Resources.SteamShutdownArgument);
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
            const int waitstep = 50;
            if (IsSteamOpen())
            {
                LogOutAuto();
                while (IsSteamOpen())
                {
                    if (timeout >= maxtimeout)
                    {
                        Popup.Show("Log out operation has timed out. Please close Steam and try again.");
                        return false;
                    }
                    Thread.Sleep(waitstep);
                    timeout += waitstep;
                }
            }
            return true;
        }

        public static string GetPath()
        {
            string path;
            try
            {
                using (var registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam"))
                    path = (string) registryKey.GetValue("SteamExe");
            }
            catch
            {
                path = "";
            }

            if (!string.IsNullOrWhiteSpace(path))
                return path;

            Popup.Show("Default Steam path could not be located.\r\n\r\nPlease enter Steam executable location.");
            var dia = new SteamPath();
            dia.ShowDialog();
            return dia.Path;
        }

        public static bool ResolvePath()
        {
            if (!FileExists(Settings.Default.SteamPath))
                Settings.Default.SteamPath = GetPath();
            return FileExists(Settings.Default.SteamPath);
        }

        public static bool FileExists(string path)
        {
            return (!string.IsNullOrWhiteSpace(path) && File.Exists(path));
        }

        public static bool IsSteamOpen()
        {
            return (Process.GetProcessesByName(Resources.Steam).Length > 0);
        }
    }
}