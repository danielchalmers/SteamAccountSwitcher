#region

using System;
using System.ComponentModel;
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

        public static bool IsFirstLaunch => Settings.Default.Launches <= 1;
        public static bool IsUpdateable => ApplicationDeployment.IsNetworkDeployed;
        private static Version ForgetUpdateVersion => Settings.Default.ForgetUpdateVersion ?? new Version(0, 0, 0, 0);

        public static void CheckForUpdatesAsync(bool auto)
        {
            UpdateCheckInfo updateInfo = null;
            var bw = new BackgroundWorker();
            bw.DoWork += delegate
            {
                if (ApplicationDeployment.IsNetworkDeployed)
                    updateInfo = ApplicationDeployment.CurrentDeployment.CheckForDetailedUpdate();
            };
            bw.RunWorkerCompleted += (sender, args) => CheckForUpdates(updateInfo, auto);
            bw.RunWorkerAsync();
        }

        public static void CheckForUpdates(bool auto)
            => CheckForUpdates(ApplicationDeployment.CurrentDeployment.CheckForDetailedUpdate(), auto);

        public static void CheckForUpdates(UpdateCheckInfo info, bool auto)
        {
            try
            {
                if (!ApplicationDeployment.IsNetworkDeployed)
                {
                    if (!auto)
                        Popup.Show(
                            "This application was not installed via ClickOnce and cannot be updated automatically.");
                    return;
                }

                if (info == null)
                {
                    if (!auto)
                        Popup.Show(
                            "An error occurred while trying to check for updates.");
                    return;
                }

                Settings.Default.LastUpdateCheck = DateTime.Now;
                if (info.UpdateAvailable)
                {
                    if (AssemblyInfo.GetVersion().Major != ForgetUpdateVersion.Major &&
                        ForgetUpdateVersion.Major == info.AvailableVersion.Major)
                        return;

                    var ad = ApplicationDeployment.CurrentDeployment;
                    ad.UpdateCompleted += (sender, args) => RestartApplication();
                    if (auto && Settings.Default.AutoUpdate)
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
                    if (auto && info.AvailableVersion == ForgetUpdateVersion)
                        return;
                    App.UpdateScheduler.Stop();

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
                                    "Cannot download the latest version of this application.\n\nPlease check your network connection, or try again later.\n\nError: " +
                                    dde);
                            }
                            break;
                    }

                    App.UpdateScheduler.Start();
                }
                else
                {
                    if (!auto)
                        Popup.Show(
                            "There are no new updates available.");
                }
            }
            catch (Exception)
            {
                if (!auto)
                    throw;
            }
        }

        public static void RestartApplication()
        {
            Process.Start(IsUpdateable ? AppPath : Application.ResourceAssembly.Location, "-restarting");
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
                registryKey?.SetValue(Resources.AppPathName, AppPath);
            else
                registryKey?.DeleteValue(Resources.AppPathName, false);
        }
    }
}