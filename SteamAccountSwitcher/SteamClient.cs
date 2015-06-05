#region

using System.Diagnostics;
using System.IO;
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

        public static string ResolvePath()
        {
            const string bit32Path = "C:\\Program Files\\Steam\\Steam.exe";
            const string bit64Path = "C:\\Program Files (x86)\\Steam\\Steam.exe";

            if (File.Exists(bit32Path))
                return bit32Path;
            if (File.Exists(bit64Path))
                return bit64Path;
            Popup.Show("Default Steam path could not be located.\r\n\r\nPlease enter Steam executable location.");
            var dia = new SteamPath();
            dia.ShowDialog();
            return dia.Path;
        }
    }
}