#region

using System;
using System.Linq;
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
            if (IsExistingInstanceRunning())
            {
                //Popup.Show("You can only run one instance at a time.");
                return false;
            }
            LoadSettings();
            if (!SteamClient.ResolvePath())
            {
                Popup.Show("Steam path could not be located. Application will now exit.");
                return false;
            }
            App.SaveTimer = new SaveTimer(Settings.Default.SaveDelay);
            LoadAccounts();
            InitMainWindow();
            if (Settings.Default.AlwaysOn)
                TrayIconHelper.CreateTrayIcon();

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
            App.SwitchWindow = new SwitchWindow();
            new WindowInteropHelper(App.SwitchWindow).EnsureHandle();
            if (Settings.Default.AlwaysOn)
            {
            }
            else
            {
                SwitchWindowHelper.ShowSwitcherWindow();
            }
        }
        
        private static bool IsExistingInstanceRunning()
        {
            bool isNewInstance;
            App.AppMutex = new Mutex(true, AssemblyInfo.Guid, out isNewInstance);
            if (App.Arguments.Contains("-restarting") || App.Arguments.Contains("-multiinstance") ||
                Settings.Default.MultiInstance || isNewInstance)
                return false;
            SingleInstanceHelper.ShowExistingInstance();
            ClickOnceHelper.ShutdownApplication();
            return true;
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

            if (App.Arguments.Contains("-reset"))
                SettingsHelper.ResetSettings();

            SettingsHelper.IncrementLaunches();
        }
    }
}