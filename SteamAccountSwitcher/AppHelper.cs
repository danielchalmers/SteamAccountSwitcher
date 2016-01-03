#region

using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Diagnostics;
using System.Windows;
using Microsoft.Win32;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    internal static class AppHelper
    {
        private static readonly string AppPath =
            IsUpdateable
                ? $"\"{Environment.GetFolderPath(Environment.SpecialFolder.Programs)}\\Daniel Chalmers\\{Resources.AppName}.appref-ms\""
                : Application.ResourceAssembly.Location;

        private static bool IsUpdateable => ApplicationDeployment.IsNetworkDeployed;

        public static bool IsFirstLaunch => Settings.Default.Launches <= 1;

        public static void RestartApplication(IEnumerable<string> arguments = null)
        {
            var args = new List<string> {"restarting"};
            if (arguments != null) args.AddRange(arguments);
            Process.Start(AppPath, string.Join(",-", args));
            ShutdownApplication();
        }

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
                registryKey?.SetValue(Resources.AppPathName, AppPath + " -systemstartup");
            else
                registryKey?.DeleteValue(Resources.AppPathName, false);
        }
    }
}