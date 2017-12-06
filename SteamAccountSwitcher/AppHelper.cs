using System;
using System.Deployment.Application;
using System.Windows;
using Microsoft.Win32;
using SteamAccountSwitcher.Properties;

namespace SteamAccountSwitcher
{
    public static class AppHelper
    {
        private static readonly string RunPath =
            IsUpdateable
                ? $"\"{Environment.GetFolderPath(Environment.SpecialFolder.Programs)}\\Daniel Chalmers\\{AssemblyInfo.Title}.appref-ms\""
                : Application.ResourceAssembly.Location;

        private static bool IsUpdateable => ApplicationDeployment.IsNetworkDeployed;

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