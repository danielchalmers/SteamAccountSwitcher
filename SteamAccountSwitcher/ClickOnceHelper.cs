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

        public static bool IsFirstLaunch => Settings.Default.Launches == 1;
        public static bool IsUpdateable => ApplicationDeployment.IsNetworkDeployed;

        public static void CheckForUpdates(bool silent = false)
        {
            if (!ApplicationDeployment.IsNetworkDeployed)
            {
                if (!silent)
                    Popup.Show("This application was not installed via ClickOnce and cannot be updated automatically.");
                return;
            }
            var ad = ApplicationDeployment.CurrentDeployment;

            UpdateCheckInfo info;
            try
            {
                info = ad.CheckForDetailedUpdate();
            }
            catch (DeploymentDownloadException dde)
            {
                if (!silent)
                    Popup.Show(
                        "The new version of the application cannot be downloaded at this time.\n\nPlease check your network connection, and try again later.\n\nError: " +
                        dde.Message);
                return;
            }
            catch (InvalidDeploymentException ide)
            {
                if (!silent)
                    Popup.Show(
                        "Cannot check for a new version of the application. The ClickOnce installation is corrupt. Please reinstall the application and try again.\n\nError: " +
                        ide.Message);
                return;
            }
            catch (InvalidOperationException ioe)
            {
                if (!silent)
                    Popup.Show(
                        "This application cannot be updated. It is likely not a ClickOnce application.\n\nError: " +
                        ioe.Message);
                return;
            }
            catch (Exception ex)
            {
                if (!silent)
                    Popup.Show(
                        "An error occurred while trying to check for updates.\n\nError: " +
                        ex.Message);
                return;
            }

            if (info.UpdateAvailable)
            {
                if (silent && Settings.Default.AutoUpdate)
                {
                    try
                    {
                        ad.UpdateAsync();
                        RestartApplication();
                    }
                    catch
                    {
                        // ignored
                    }
                    return;
                }
                if (silent && info.AvailableVersion == Settings.Default.ForgetUpdateVersion) return;
                UpdateChecker.Stop();

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
                            RestartApplication();
                        }
                        catch (DeploymentDownloadException dde)
                        {
                            Popup.Show(
                                "Cannot install the latest version of the application.\n\nPlease check your network connection, or try again later.\n\nError: " +
                                dde);
                        }
                        break;
                }
                UpdateChecker.Start();
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