#region

using System.Windows.Media;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    public class Account
    {
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public SolidColorBrush Color { get; set; } = new SolidColorBrush(Settings.Default.ButtonDefaultColor);
    }
}