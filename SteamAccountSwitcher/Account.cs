using System.Windows.Media;

namespace SteamAccountSwitcher
{
    public class Account
    {
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public Brush Color { get; set; }
        public string ColorText { get; set; }
    }
}