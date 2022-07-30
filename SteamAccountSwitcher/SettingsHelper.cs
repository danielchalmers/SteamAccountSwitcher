using System.Collections.Generic;
using Newtonsoft.Json;
using SteamAccountSwitcher.Properties;

namespace SteamAccountSwitcher
{
    public static class SettingsHelper
    {
        public static void SaveSettings()
        {
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

        public static AccountCollection DeserializeAccounts(string accounts)
        {
            try
            {
                var deobfuscated = new Obfuscator().Deobfuscate(accounts);
                var deserialized = JsonConvert.DeserializeObject<AccountCollection>(deobfuscated);

                return deserialized;
            }
            catch
            {
                return null;
            }
        }
    }
}