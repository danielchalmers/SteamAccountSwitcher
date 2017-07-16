using System;
using System.Windows.Media;
using SteamAccountSwitcher.Properties;

namespace SteamAccountSwitcher
{
    public class Account : ICloneable
    {
        public string DisplayName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Color Color { get; set; } = Settings.Default.DefaultButtonColor;
        public Color TextColor { get; set; } = Settings.Default.DefaultButtonTextColor;
        public DateTime AddDate { get; set; } = DateTime.Now;
        public DateTime LastModifiedDate { get; set; } = DateTime.Now;
        public string Arguments { get; set; } = string.Empty;

        public object Clone()
        {
            return MemberwiseClone();
        }

        public string GetDisplayName()
        {
            return string.IsNullOrWhiteSpace(DisplayName)
                ? Username
                : DisplayName;
        }
    }
}