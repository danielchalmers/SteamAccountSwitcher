using System.ComponentModel;
using System.Windows.Media;
using SteamAccountSwitcher.Properties;

namespace SteamAccountSwitcher
{
    public record class Account
    {
        public string DisplayName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Color Color { get; set; } = Settings.Default.DefaultButtonColor;
        public Color TextColor { get; set; } = Settings.Default.DefaultButtonTextColor;
        public string Arguments { get; set; } = string.Empty;

        public override string ToString() =>
            string.IsNullOrWhiteSpace(DisplayName)
                ? Username
                : DisplayName;

        public void SwitchTo()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += delegate
            {
                SteamClient.LogoutWithTimeout();
                SteamClient.Login(this);
            };

            worker.RunWorkerAsync();
        }

        public static Account ExampleAccount { get; } = new()
        {
            DisplayName = "Example",
            Username = "username",
            Password = "password"
        };
    }
}