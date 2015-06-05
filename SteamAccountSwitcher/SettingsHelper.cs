#region

using System.Collections.Generic;
using Newtonsoft.Json;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    internal class SettingsHelper
    {
        public static string SerializeAccounts(List<Account> accounts)
        {
            return new Encryption().Encrypt(JsonConvert.SerializeObject(accounts));
        }

        public static List<Account> DeserializeAccounts()
        {
            return string.IsNullOrWhiteSpace(Settings.Default.Accounts)
                ? new List<Account>()
                : JsonConvert.DeserializeObject<List<Account>>(new Encryption().Decrypt(Settings.Default.Accounts));
        }

        public static void OpenOptions(AccountHandler accountHandler)
        {
            // Open options window.
            var dialog = new Options();
            dialog.ShowDialog();
            accountHandler.Refresh();
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