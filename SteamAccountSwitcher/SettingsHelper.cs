using System.Collections.Generic;
using System.Windows;
using Newtonsoft.Json;
using SteamAccountSwitcher.Properties;

namespace SteamAccountSwitcher
{
    public static class SettingsHelper
    {
        public static void SaveSettings()
        {
            if (!App.HasInitialized)
            {
                return;
            }
            Settings.Default.Accounts = SerializeAccounts(App.Accounts);
            Settings.Default.Save();
        }

        public static void UpgradeSettings()
        {
            if (Settings.Default.MustUpgrade)
            {
                Settings.Default.Upgrade();
                Settings.Default.MustUpgrade = false;
                Settings.Default.Save();
            }
        }

        public static string SerializeAccounts(IEnumerable<Account> accounts)
        {
            // Serialize account list to JSON.
            var serialized = JsonConvert.SerializeObject(accounts);

            // Obfuscate the serialized accounts.
            var obfuscated = new Encryption().Encrypt(serialized);

            // Return the obfuscated accounts.
            // Deserialization must occur in reverse order (deobfuscate then deserialize).
            return obfuscated;
        }

        public static IEnumerable<Account> DeserializeAccounts(string accounts)
        {
            // If the accounts data is empty, return a new account list.
            if (string.IsNullOrEmpty(accounts))
            {
                return new List<Account>();
            }

            // Deobfuscate JSON accounts string.
            var deobfuscated = new Encryption().Decrypt(accounts);

            // Deserialize deobfuscated JSON string.
            var deserialized = JsonConvert.DeserializeObject<List<Account>>(deobfuscated);

            // If the deserialization was successful, return the result, otherwise return a new account list.
            return deserialized ?? new List<Account>();
        }

        public static void OpenOptions()
        {
            var alwaysOn = Settings.Default.AlwaysOn;

            var dialog = new Options();
            dialog.ShowDialog();

            if (alwaysOn != Settings.Default.AlwaysOn)
            {
                if (Settings.Default.AlwaysOn)
                {
                    TrayIconHelper.CreateTrayIcon();
                    App.HideMainWindow();
                }
                else
                {
                    App.ShowMainWindow();
                }
            }
            else
            {
                if (Settings.Default.AlwaysOn)
                {
                    TrayIconHelper.RefreshTrayIconMenu();
                }
            }
        }
    }
}