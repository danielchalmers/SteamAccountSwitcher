#region

using System;
using System.Diagnostics;
using System.Windows;
using Microsoft.Win32;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    internal class ClickOnceHelper
    {
        public static readonly string AppPath =
            $"\"{Environment.GetFolderPath(Environment.SpecialFolder.Programs)}\\Daniel Chalmers\\{Resources.AppName}.appref-ms\"";

        public static bool IsFirstLaunch => Settings.Default.Launches <= 1;

        public static void RestartApplication(string args = "")
        {
            Process.Start(UpdateHelper.IsUpdateable ? AppPath : Application.ResourceAssembly.Location,
                $"-restarting {args}");
            ShutdownApplication();
        }

        public static void ShutdownApplication()
        {
            Application.Current.Shutdown();
        }

        public static void RunOnStartup(bool runonstartup)
        {
            var registryKey = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (runonstartup)
                registryKey?.SetValue(Resources.AppPathName, AppPath + " -systemstartup");
            else
                registryKey?.DeleteValue(Resources.AppPathName, false);
        }
    }
}