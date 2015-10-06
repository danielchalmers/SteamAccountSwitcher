using System;
using System.ComponentModel;
using System.Deployment.Application;
using SteamAccountSwitcher.Properties;

namespace SteamAccountSwitcher
{
    internal class UpdateHelper
    {
        public static bool IsUpdateable => ApplicationDeployment.IsNetworkDeployed;
        private static Version ForgetUpdateVersion => Settings.Default.ForgetUpdateVersion ?? new Version(0, 0, 0, 0);

        public static void CheckForUpdatesAsync(bool auto)
        {
            if (FullscreenHelper.DoesAnyMonitorHaveFullscreenApp())
                return;

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

        private static void CheckForUpdates(UpdateCheckInfo info, bool auto)
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
                    ad.UpdateCompleted += (sender, args) => ClickOnceHelper.RestartApplication();
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
                    App.UpdateScheduler?.Stop();

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

                    App.UpdateScheduler?.Start();
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
    }
}