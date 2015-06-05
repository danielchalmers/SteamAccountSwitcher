#region

using System;
using System.Deployment.Application;
using System.Windows;
using Microsoft.Win32;

#endregion

namespace SteamAccountSwitcher
{
    internal class ClickOnceHelper
    {
        public static bool IsFirstRun
            => (ApplicationDeployment.IsNetworkDeployed && ApplicationDeployment.CurrentDeployment.IsFirstRun);

        public static void CheckForUpdates()
        {
            if (!ApplicationDeployment.IsNetworkDeployed)
            {
                MessageBox.Show("This version was not installed via ClickOnce and cannot be updated automatically.");
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
                MessageBox.Show(
                    "The new version of the application cannot be downloaded at this time. \n\nPlease check your network connection, or try again later. Error: " +
                    dde.Message);
                return;
            }
            catch (InvalidDeploymentException ide)
            {
                MessageBox.Show(
                    "Cannot check for a new version of the application. The ClickOnce deployment is corrupt. Please redeploy the application and try again. Error: " +
                    ide.Message);
                return;
            }
            catch (InvalidOperationException ioe)
            {
                MessageBox.Show(
                    "This application cannot be updated. It is likely not a ClickOnce application. Error: " +
                    ioe.Message);
                return;
            }

            if (info.UpdateAvailable)
            {
                var doUpdate = true;

                if (!info.IsUpdateRequired)
                {
                    var dr = MessageBox.Show("An update is available. Would you like to update the application now?",
                        "Update Available", MessageBoxButton.OKCancel);
                    if (dr != MessageBoxResult.OK)
                    {
                        doUpdate = false;
                    }
                }
                else
                {
                    // Display a message that the app MUST reboot. Display the minimum required version.
                    MessageBox.Show("This application has detected a mandatory update from your current " +
                                    "version to version " + info.MinimumRequiredVersion +
                                    ". The application will now install the update and shutdown.",
                        "Update Available", MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }

                if (doUpdate)
                {
                    try
                    {
                        ad.Update();
                        MessageBox.Show("The application has been upgraded, and will now shutdown.");
                        Application.Current.Shutdown();
                    }
                    catch (DeploymentDownloadException dde)
                    {
                        MessageBox.Show(
                            "Cannot install the latest version of the application. \n\nPlease check your network connection, or try again later. Error: " +
                            dde);
                    }
                }
            }
        }

        public static void RunOnStartup(bool run)
        {
            // The path to the key where Windows looks for startup applications
            var rkApp = Registry.CurrentUser.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", run);

            //Path to launch shortcut
            var startPath = Environment.GetFolderPath(Environment.SpecialFolder.Programs)
                            + @"\Daniel Chalmers\SteamAccountSwitcher.appref-ms";

            rkApp?.SetValue("SteamAccountSwitcher", startPath);
        }
    }
}