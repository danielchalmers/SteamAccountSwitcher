#region

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using Microsoft.Win32;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    internal static class SteamClient
    {
        private static void StartSteam(string args = "")
        {
            if (!ResolvePath())
            {
                Popup.Show("Steam.exe could not be found.\n\nPlease enter the correct path in options.",
                    image: MessageBoxImage.Warning);
                return;
            }
            Process.Start(Settings.Default.SteamPath, args);
        }

        public static void Launch()
        {
            StartSteam();
        }

        public static void LogIn(Account account, bool onStart = false)
        {
            var args = new List<string>();

            args.Add($"{Resources.SteamLoginArgument} \"{account.Username}\" \"{account.Password}\"");
            args.Add(account.Arguments);
            if (Settings.Default.BigPictureMode)
                args.Add(Resources.SteamBigPictureArg);

            if ((Settings.Default.StartSteamMinimized && Settings.Default.StartSteamMinimizedOnlyOnStartup && onStart) ||
                (Settings.Default.StartSteamMinimized && !Settings.Default.StartSteamMinimizedOnlyOnStartup))
                args.Add(Resources.SteamSilentArg);

            args.Add(Settings.Default.SteamLaunchArgs);

            StartSteam(string.Join(" ", args));
        }

        private static void LogOut()
        {
            if (!IsSteamOpen())
                return;
            StartSteam(Resources.SteamShutdownArgument);
        }

        private static Process GetSteamProcess()
        {
            var processes = Process.GetProcessesByName(Resources.Steam);
            return processes.Length > 0 ? processes[0] : null;
        }

        private static void ForceClose()
        {
            GetSteamProcess()?.CloseMainWindow();
        }

        private static string GetSteamTitle()
        {
            return GetSteamProcess()?.MainWindowTitle;
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
                        ForceClose();
                        return false;
                    }
                    Thread.Sleep(waitstep);
                    timeout += waitstep;
                }
            }
            return true;
        }

        private static string GetPathFromRegistry()
        {
            string path;
            try
            {
                using (var registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam"))
                    path = registryKey?.GetValue("SteamExe").ToString();
            }
            catch
            {
                path = "";
            }
            return path;
        }

        private static bool ResolvePath()
        {
            if (!File.Exists(Settings.Default.SteamPath))
                Settings.Default.SteamPath = GetPathFromRegistry();
            return File.Exists(Settings.Default.SteamPath);
        }

        private static bool IsSteamOpen()
        {
            return Process.GetProcessesByName(Resources.Steam).Length > 0;
        }
    }
}