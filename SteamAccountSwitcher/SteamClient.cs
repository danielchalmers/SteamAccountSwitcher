using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
                Popup.Show(
                    "Steam.exe could not be found.\n\n" +
                    "Please enter the correct path in options.",
                    image: MessageBoxImage.Warning);
                return;
            }
            Process.Start(Settings.Default.SteamPath, args);
        }

        public static void Login(Account account, bool onStart = false)
        {
            Launch(string.Join(" ", GetLoginArguments(account, onStart)));
        }

        private static IEnumerable<string> GetLoginArguments(Account account, bool onStart = false)
        {
            // Login.
            yield return Resources.SteamLoginArgument;
            yield return account.Username;
            yield return account.Password;

            // Per-account arguments.
            yield return account.Arguments;

            // Global arguments.
            yield return Settings.Default.SteamLaunchArgs;

            // Minimized.
            var minimize = Settings.Default.StartSteamMinimized;
            var minimizeOnStartup = Settings.Default.StartSteamMinimizedOnlyOnStartup;
            if ((minimize && minimizeOnStartup && onStart) || (minimize && !minimizeOnStartup))
            {
                yield return Resources.SteamSilentArg;
            }
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
            return Process.GetProcessesByName(Resources.Steam).FirstOrDefault();
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