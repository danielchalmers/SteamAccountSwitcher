#region

using System.Diagnostics;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    internal class SteamClient
    {
        public static void LogIn(Account account)
        {
            Process.Start(Settings.Default.SteamPath, $"-login \"{account.Username}\" \"{account.Password}\"");
        }

        public static void LogOut()
        {
            Process.Start(Settings.Default.SteamPath, "-shutdown");
        }
    }
}