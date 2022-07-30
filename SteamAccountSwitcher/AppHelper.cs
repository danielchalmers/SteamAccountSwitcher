using System;
using System.Windows;
using Microsoft.Win32;
using SteamAccountSwitcher.Properties;

namespace SteamAccountSwitcher
{
    public static class AppHelper
    {
        private static readonly string RunPath = Application.ResourceAssembly.Location;

        public static void Shutdown()
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
                registryKey?.SetValue(Resources.AppPathName, $"{RunPath} -systemstartup");
            }
            else
            {
                registryKey?.DeleteValue(Resources.AppPathName, false);
            }
        }
    }
}