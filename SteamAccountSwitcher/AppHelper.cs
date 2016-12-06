#region

using System;
using System.Deployment.Application;
using System.Windows;
using Microsoft.Win32;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    public static class AppHelper
    {
        private static readonly string AppPath =
            IsUpdateable
                ? $"\"{Environment.GetFolderPath(Environment.SpecialFolder.Programs)}\\Daniel Chalmers\\{Resources.AppName}.appref-ms\""
                : Application.ResourceAssembly.Location;

        private static bool IsUpdateable => ApplicationDeployment.IsNetworkDeployed;
        public static bool IsFirstLaunch => Settings.Default.Launches <= 1;

        public static void ShutdownApplication()
        {
            App.IsShuttingDown = true;
            Application.Current.Shutdown();
        }

        public static void SetRunOnStartup(bool runOnStartup)
        {
            var registryKey = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (runOnStartup)
            {
                registryKey?.SetValue(Resources.AppPathName, $"{AppPath} -systemstartup");
            }
            else
            {
                registryKey?.DeleteValue(Resources.AppPathName, false);
            }
        }
    }
}