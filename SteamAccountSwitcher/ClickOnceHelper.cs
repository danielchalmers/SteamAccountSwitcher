#region

using System;
using System.Deployment.Application;
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

        public static bool IsFirstLaunch => Settings.Default.Launches == 1 || Settings.Default.Launches == 0;
        public static bool IsUpdateable => ApplicationDeployment.IsNetworkDeployed;

        public static void CheckForUpdates(bool silent = false)
        {
            new UpdateCheck().CheckForUpdates(x => CheckForUpdates(x, silent));
        }

        private static void CheckForUpdates(UpdateCheckInfo info, bool silent = false)
        {
            if (!ApplicationDeployment.IsNetworkDeployed)
            {
                if (!silent)
                    Popup.Show("This application was not installed via ClickOnce and cannot be updated automatically.");
                return;
            }

            if (info == null)
            {
                if (!silent)
                    Popup.Show(
                        "An error occurred while trying to check for updates.");
                return;
            }

            if (info.UpdateAvailable)
            {
                if (AssemblyInfo.GetVersion().Major != Settings.Default.ForgetUpdateVersion.Major &&
                    Settings.Default.ForgetUpdateVersion.Major == info.AvailableVersion.Major)
                    return;

                var ad = ApplicationDeployment.CurrentDeployment;
                ad.UpdateCompleted += (sender, args) => RestartApplication();
                if (silent && Settings.Default.AutoUpdate)
                {
                    try
                    {
                        ad.UpdateAsync();
                    }
                    catch
                    {
                        // ignored
                    }
                    return;
                }
                if (silent && info.AvailableVersion == Settings.Default.ForgetUpdateVersion) return;
                UpdateCheckScheduler.Stop();

                var updateDialog = new UpdatePrompt(info.AvailableVersion, info.IsUpdateRequired);
                updateDialog.ShowDialog();

                switch (updateDialog.UserResponse)
                {
                    case UpdatePrompt.UpdateResponse.RemindNever:
                        Settings.Default.ForgetUpdateVersion = info.AvailableVersion;
                        break;
                    case UpdatePrompt.UpdateResponse.UpdateNow:
                        try
                        {
                            ad.UpdateAsync();
                        }
                        catch (DeploymentDownloadException dde)
                        {
                            Popup.Show(
                                "Cannot install the latest version of the application.\n\nPlease check your network connection, or try again later.\n\nError: " +
                                dde);
                        }
                        break;
                }
                UpdateCheckScheduler.Start();
            }
            else
            {
                if (!silent)
                    Popup.Show(
                        "There are no new updates available.");
            }
        }

        public static void RestartApplication()
        {
            Process.Start(IsUpdateable ? AppPath : Application.ResourceAssembly.Location, "--restarting");
            Application.Current.Shutdown();
        }

        public static void RunOnStartup(bool runonstartup)
        {
            var registryKey = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (runonstartup)
                registryKey?.SetValue(Resources.AppPathName, AppPath);
            else
                registryKey?.DeleteValue(Resources.AppPathName, false);
        }
    }
}