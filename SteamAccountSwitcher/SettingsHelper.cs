#region

using System;
using System.Windows;
using Newtonsoft.Json;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    internal class SettingsHelper
    {
        public static string SerializeAccounts(AccountData accounts)
        {
            try
            {
                return new Encryption().Encrypt(JsonConvert.SerializeObject(accounts));
            }
            catch (Exception)
            {
                //Popup.Show("Could not save account data.", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            return string.Empty;
        }

        public static AccountData DeserializeAccounts()
        {
            try
            {
                return string.IsNullOrWhiteSpace(Settings.Default.AccountData)
                    ? new AccountData()
                    : JsonConvert.DeserializeObject<AccountData>(new Encryption().Decrypt(Settings.Default.AccountData));
            }
            catch (Exception)
            {
                Popup.Show("Existing account data is corrupt.\r\n\r\nAll accounts cleared.", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            return null;
        }

        public static void OpenOptions(AccountHandler accountHandler)
        {
            var alwaysOn = Settings.Default.AlwaysOn;

            // Open options window.
            var dialog = new Options(accountHandler);
            dialog.ShowDialog();

            if (alwaysOn != Settings.Default.AlwaysOn)
                if (Settings.Default.AlwaysOn)
                    accountHandler._hideWindow();
                else
                    accountHandler._showWindow();

            accountHandler.Refresh();
            SaveSettings(accountHandler);
        }

        public static void SaveSettings(AccountHandler accountHandler)
        {
            // Save settings.
            accountHandler.SaveAccounts();
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
    }
}