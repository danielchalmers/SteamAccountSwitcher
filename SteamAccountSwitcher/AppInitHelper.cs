using System.Linq;
using System.Threading;
using SteamAccountSwitcher.Properties;

namespace SteamAccountSwitcher
{
    public static class AppInitHelper
    {
        public static bool Initialize()
        {
            if (IsExistingInstanceRunning())
            {
                return false;
            }

            SettingsHelper.UpgradeSettings();
            LoadAccounts();

            App.TrayIcon = (MyTaskbarIcon)App.Current.FindResource("TrayIcon");

            if (!App.Arguments.Contains("-systemstartup"))
                App.TrayIcon.ShowRunningInTrayNotification();

            LaunchStartAccount();
            return true;
        }

        private static bool IsExistingInstanceRunning()
        {
            App.AppMutex = new Mutex(true, AssemblyInfo.Guid, out var isNewInstance);

            if (isNewInstance)
                return false;

            AppHelper.Shutdown();
            return true;
        }

        private static void LoadAccounts()
        {
            if (string.IsNullOrEmpty(Settings.Default.Accounts))
            {
                Settings.Default.Accounts = AccountDataHelper.DefaultData();
            }

            AccountDataHelper.ReloadData();
        }

        private static void LaunchStartAccount()
        {
            if (!string.IsNullOrEmpty(Settings.Default.OnStartLoginName) &&
                App.Arguments.Contains("-systemstartup"))
            {
                App.Accounts.FirstOrDefault(x => x.Username == Settings.Default.OnStartLoginName)?.SwitchTo(onStart: true);
            }
        }
    }
}