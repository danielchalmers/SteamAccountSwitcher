#region

using System.Collections.Generic;

#endregion

namespace SteamAccountSwitcher
{
    public class AccountData
    {
        public int Version { get; set; } = 2;
        public List<Account> Accounts { get; set; } = new List<Account>();
    }
}