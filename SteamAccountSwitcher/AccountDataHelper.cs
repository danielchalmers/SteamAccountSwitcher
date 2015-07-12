#region

using System.Collections.Generic;

#endregion

namespace SteamAccountSwitcher
{
    internal class AccountDataHelper
    {
        public static string DefaultData()
        {
            return SettingsHelper.SerializeAccounts(new List<Account>
            {
                new Account
                {
                    DisplayName = "Example",
                    Username = "username",
                    Password = "password"
                }
            });
        }

        public static void ReloadData()
        {
            App.Accounts = SettingsHelper.DeserializeAccounts();
        }
    }
}