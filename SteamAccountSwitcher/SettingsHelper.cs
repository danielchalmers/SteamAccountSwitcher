#region

using System;
using System.Collections.Generic;
using System.Windows;
using Newtonsoft.Json;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    internal class SettingsHelper
    {
        public static string SerializeAccounts(List<Account> accounts)
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

        public static List<Account> DeserializeAccounts()
        {
            try
            {
                return string.IsNullOrWhiteSpace(Settings.Default.Accounts)
                    ? new List<Account>()
                    : JsonConvert.DeserializeObject<List<Account>>(new Encryption().Decrypt(Settings.Default.Accounts));
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
            accountHandler.SaveAccounts();

            // Open options window.
            var dialog = new Options();
            dialog.ShowDialog();

            accountHandler.ReloadAccounts();
            accountHandler.Refresh();
        }

        public static void SaveSettings(AccountHandler accountHandler)
        {
            // Save settings.
            accountHandler.SaveAccounts();
            Settings.Default.Save();

            ClickOnceHelper.RunOnStartup(Settings.Default.AlwaysOn);
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