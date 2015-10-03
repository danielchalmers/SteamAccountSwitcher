#region

using System;
using System.Threading;
using System.Windows.Interop;
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
            InitMainWindow();
            return true;
        }

        private static void InitMainWindow()
        {
            App.SwitchWindow = new MainWindow();
            if (Settings.Default.AlwaysOn)
            {
                var helper = new WindowInteropHelper(App.SwitchWindow);
                helper.EnsureHandle();
                App.SwitchWindow.ShowRunningInTrayBalloon();
            }
            else
            {
                App.SwitchWindow.Show();
            }
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

        private static bool IsExistingInstanceRunning()
        {
            bool isNewInstance;
            App.AppMutex = new Mutex(true, AssemblyInfo.GetGuid(), out isNewInstance);
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