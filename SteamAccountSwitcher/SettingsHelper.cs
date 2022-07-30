using System.Collections.Generic;
using Newtonsoft.Json;
using SteamAccountSwitcher.Properties;

namespace SteamAccountSwitcher
{
    public static class SettingsHelper
    {
        public static void SaveSettings()
        {
            if (!App.HasInitialized)
                return;

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
            var serialized = JsonConvert.SerializeObject(accounts);
            var obfuscated = new Obfuscator().Obfuscate(serialized);
            return obfuscated;
        }

        public static IEnumerable<Account> DeserializeAccounts(string accounts)
        {
            if (string.IsNullOrEmpty(accounts))
                return new List<Account>();

            var deobfuscated = new Obfuscator().Deobfuscate(accounts);
            var deserialized = JsonConvert.DeserializeObject<List<Account>>(deobfuscated);

            return deserialized ?? new List<Account>();
        }
    }
}