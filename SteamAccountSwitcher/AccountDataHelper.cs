#region

using System.Collections.Generic;
using Newtonsoft.Json;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    internal class AccountDataHelper
    {
        public static string DefaultData()
        {
            return SettingsHelper.SerializeAccounts(new AccountData
            {
                Accounts = new List<Account>
                {
                    new Account
                    {
                        DisplayName = "Example",
                        Username = "username",
                        Password = "password"
                    }
                }
            });
        }

        public static void UpgradeAccounts()
        {
            if (!string.IsNullOrWhiteSpace(Settings.Default.Accounts))
                Settings.Default.AccountData =
                    SettingsHelper.SerializeAccounts(new AccountData
                    {
                        Accounts =
                            JsonConvert.DeserializeObject<List<Account>>(
                                new Encryption().Decrypt(Settings.Default.Accounts))
                    });
        }
    }
}