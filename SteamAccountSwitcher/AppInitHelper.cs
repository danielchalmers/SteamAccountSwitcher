#region

using System;
using System.Linq;
using System.Threading;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    internal class AppInitHelper
    {
        public static bool Initialize()
        {
            App.HelperWindow = new HelperWindow();
            if (IsExistingInstanceRunning())
            {
                Popup.Show("You can only run one instance at a time.");
                return false;
            }
            LoadSettings();
            if (!SteamClient.ResolvePath())
            {
                Popup.Show("Steam path could not be located. Application will now exit.");
                return false;
            }
            StartScheduledTasks();
            LoadAccounts();
            if (Settings.Default.AlwaysOn)
                TrayIconHelper.CreateTrayIcon();
            InitMainWindow();

            LaunchStartAccount();

            return true;
        }

        private static void LaunchStartAccount()
        {
            if (!string.IsNullOrWhiteSpace(Settings.Default.OnStartLoginName) && Settings.Default.AlwaysOn &&
                App.Arguments.Contains("-systemstartup"))
            {
                foreach (
                    var account in
                        App.Accounts.Where(x => x.Username == Settings.Default.OnStartLoginName)
                    )
                {
                    account.SwitchTo(true);
                    break;
                }
            }
        }

        private static void InitMainWindow()
        {
            if (Settings.Default.AlwaysOn)
            {
                TrayIconHelper.ShowRunningInTrayBalloon();
            }
            else
            {
                SwitchWindowHelper.ShowSwitcherWindow();
            }
        }

        private static void StartScheduledTasks()
        {
            if (Settings.Default.UpdateCheckIntervalMinutes > 0)
            {
                App.UpdateScheduler = new TaskScheduler();
                App.UpdateScheduler.ScheduleTask(() =>
                    UpdateHelper.CheckForUpdatesAsync(true),
                    (Settings.Default.CheckForUpdates && UpdateHelper.IsUpdateable),
                    TimeSpan.FromMinutes(Settings.Default.UpdateCheckIntervalMinutes));
                App.UpdateScheduler.Start();
            }
        }

        private static bool IsExistingInstanceRunning()
        {
            bool isNewInstance;
            App.AppMutex = new Mutex(true, AssemblyInfo.Guid, out isNewInstance);
            return !(App.Arguments.Contains("-restarting") || App.Arguments.Contains("-multiinstance") ||
                     Settings.Default.MultiInstance || isNewInstance);
        }

        private static void LoadAccounts()
        {
            if (ClickOnceHelper.IsFirstLaunch && Settings.Default.Accounts == string.Empty)
                Settings.Default.Accounts = AccountDataHelper.DefaultData();

            AccountDataHelper.ReloadData();
        }

        private static void LoadSettings()
        {
            SettingsHelper.UpgradeSettings();

            if (Settings.Default.ResetSettingsOnNextLaunch || App.Arguments.Contains("-reset"))
                SettingsHelper.ResetSettings();

            SettingsHelper.IncrementLaunches();
        }
    }
}