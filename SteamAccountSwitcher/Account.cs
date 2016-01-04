#region

using System;
using System.Windows.Media;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    public class Account : ICloneable
    {
        public string DisplayName { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public Color Color { get; set; } = Settings.Default.DefaultButtonColor;
        public Color TextColor { get; set; } = Settings.Default.DefaultButtonTextColor;
        public DateTime AddDate { get; set; } = DateTime.Now;
        public DateTime LastModifiedDate { get; set; } = DateTime.Now;

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