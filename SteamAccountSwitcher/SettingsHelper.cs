#region

using System.Collections.Generic;
using System.Windows;
using Newtonsoft.Json;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    internal static class SettingsHelper
    {
        public static string SerializeAccounts(IEnumerable<Account> accounts)
        {
            return new Encryption().Encrypt(JsonConvert.SerializeObject(accounts));
        }

        public static IEnumerable<Account> DeserializeAccounts(string data)
        {
            return
                JsonConvert.DeserializeObject<List<Account>>(new Encryption().Decrypt(data)) ??
                new List<Account>();
        }

        public static void OpenOptions()
        {
            var alwaysOn = Settings.Default.AlwaysOn;

            // Open options window.
            var dialog = new Options();
            dialog.ShowDialog();

            if (alwaysOn != Settings.Default.AlwaysOn)
            {
                if (Settings.Default.AlwaysOn)
                {
                    TrayIconHelper.CreateTrayIcon();
                    SwitchWindowHelper.HideSwitcherWindow();
                }
                else
                {
                    SwitchWindowHelper.ShowSwitcherWindow();
                }
            }

            SaveSettings();
        }

        public static void SaveSettings()
        {
            // Save settings.
            Settings.Default.Accounts = SerializeAccounts(App.Accounts);
            Settings.Default.Save();
        }

        public static void UpgradeSettings()
        {
            // Upgrade settings from old version.
            if (Settings.Default.MustUpgrade)
            {
                Settings.Default.Upgrade();
                Settings.Default.MustUpgrade = false;
                Settings.Default.Save();
            }
        }

        public static void IncrementLaunches()
        {
            Settings.Default.Launches++;
        }

        public static void ResetSettings(bool msg = true)
        {
            if (msg && Popup.Show(
                "Are you sure you want to reset ALL settings (including accounts)?\n\nThis cannot be undone.",
                MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.No)
                return;
            Settings.Default.Reset();
            Settings.Default.MustUpgrade = false;
            Settings.Default.Accounts = AccountDataHelper.DefaultData();
            AccountDataHelper.ReloadData();
            Popup.Show("All settings have been restored to default.");
        }
    }
}