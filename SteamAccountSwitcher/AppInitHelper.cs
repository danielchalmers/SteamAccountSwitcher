#region

using System;
using System.Threading;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    internal class AppInitHelper
    {
        public static void Initialize()
        {
            App.HelperWindow = new HelperWindow();
            CheckSingleInstanceStatus();
            LoadSettings();
            SteamClient.ResolvePathShutdown();
            StartScheduledTasks();
            LoadAccounts();
        }

        private static void StartScheduledTasks()
        {
            if (Settings.Default.UpdateCheckIntervalMinutes > 0)
            {
                App.UpdateScheduler = new TaskScheduler();
                App.UpdateScheduler.ScheduleTask(() =>
                    ClickOnceHelper.CheckForUpdatesAsync(true),
                    (Settings.Default.CheckForUpdates && ClickOnceHelper.IsUpdateable),
                    TimeSpan.FromMinutes(Settings.Default.UpdateCheckIntervalMinutes));
                App.UpdateScheduler.Start();
            }
        }

        private static void CheckSingleInstanceStatus()
        {
            bool aIsNewInstance;
            App.AppMutex = new Mutex(true, AssemblyInfo.GetGuid(), out aIsNewInstance);
            if (!App.Arguments.Contains("-restarting") && !App.Arguments.Contains("-multiinstance") && !Settings.Default.MultiInstance && !aIsNewInstance)
            {
                Popup.Show("You can only run one instance at a time.");
                ClickOnceHelper.ShutdownApplication();
            }
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