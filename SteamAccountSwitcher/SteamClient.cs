using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using Microsoft.Win32;
using SteamAccountSwitcher.Properties;

namespace SteamAccountSwitcher
{
    public static class SteamClient
    {
        public static void Launch(string args = "")
        {
            if (!ResolvePath())
            {
                Popup.Show("Steam.exe could not be found.\n\n" +
                    "Please enter the correct path in options.",
                    image: MessageBoxImage.Warning);
                return;
            }
            Process.Start(Settings.Default.SteamPath, args);
        }

        public static void Login(Account account, bool onStart = false)
        {
            var args = new List<string>();

            args.Add($"{Resources.SteamLoginArgument} \"{account.Username}\" \"{account.Password}\"");
            args.Add(account.Arguments);
            if (Settings.Default.BigPictureMode)
            {
                args.Add(Resources.SteamBigPictureArg);
            }

            if ((Settings.Default.StartSteamMinimized && Settings.Default.StartSteamMinimizedOnlyOnStartup && onStart) ||
                (Settings.Default.StartSteamMinimized && !Settings.Default.StartSteamMinimizedOnlyOnStartup))
            {
                args.Add(Resources.SteamSilentArg);
            }

            args.Add(Settings.Default.SteamLaunchArgs);

            Launch(string.Join(" ", args));
        }

        public static void Logout()
        {
            if (!IsRunning())
            {
                return;
            }
            if (GetWindowTitle() == Resources.SteamNotLoggedInTitle)
            {
                ForceClose();
            }
            else
            {
                Launch(Resources.SteamShutdownArgument);
            }
        }

        public static bool LogoutWithTimeout()
        {
            var timeout = 0;
            var maxTimeout = Settings.Default.SteamLogoutTimeoutMax;
            var interval = Settings.Default.SteamLogoutTimeoutInterval;
            Logout();
            while (IsRunning())
            {
                if (timeout >= maxTimeout)
                {
                    ForceClose();
                    return false;
                }
                timeout += interval;
                Thread.Sleep(interval);
            }
            return true;
        }

        private static void ForceClose()
        {
            GetProcess()?.CloseMainWindow();
        }

        private static Process GetProcess()
        {
            var steamProcesses = Process.GetProcessesByName(Resources.Steam);
            return steamProcesses.Length > 0 ? steamProcesses[0] : null;
        }

        private static string GetWindowTitle()
        {
            return GetProcess()?.MainWindowTitle;
        }

        private static string GetPathFromRegistry()
        {
            string path;
            try
            {
                using (var registryKey = Registry.CurrentUser.OpenSubKey(Resources.SteamRegistryDirectoryPath))
                {
                    path = registryKey?.GetValue(Resources.SteamRegistryExecutableName).ToString();
                }
            }
            catch
            {
                path = string.Empty;
            }
            return path;
        }

        private static bool ResolvePath()
        {
            if (!File.Exists(Settings.Default.SteamPath))
            {
                Settings.Default.SteamPath = GetPathFromRegistry();
            }
            return File.Exists(Settings.Default.SteamPath);
        }

        private static bool IsRunning()
        {
            return GetProcess() != null;
        }
    }
}