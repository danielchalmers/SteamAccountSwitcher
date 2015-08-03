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
            CheckSingleInstanceStatus();
            LoadSettings();
            SteamClient.ResolvePathShutdown();
            StartScheduledTasks();
            LoadAccounts();
        }

        private static void StartScheduledTasks()
        {
            App.UpdateScheduler = new TaskScheduler();
            App.UpdateScheduler.ScheduleTask(() =>
                ClickOnceHelper.CheckForUpdates(true),
                (Settings.Default.CheckForUpdates && ClickOnceHelper.IsUpdateable),
                TimeSpan.FromMinutes(Settings.Default.UpdateCheckIntervalMinutes));
            App.UpdateScheduler.Start();
        }

        private static void CheckSingleInstanceStatus()
        {
            bool aIsNewInstance;
            App.AppMutex = new Mutex(true, AssemblyInfo.GetGuid(), out aIsNewInstance);
            if (!App.Arguments.Contains("-restarting") && !App.Arguments.Contains("-multiinstance") && !aIsNewInstance)
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