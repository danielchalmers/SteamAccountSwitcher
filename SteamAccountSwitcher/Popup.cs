#region

using System.Windows;

#endregion

namespace SteamAccountSwitcher
{
    internal class Popup
    {
        public static MessageBoxResult Show(string text, MessageBoxButton btn = MessageBoxButton.OK,
            MessageBoxImage img = MessageBoxImage.Information, MessageBoxResult defaultbtn = MessageBoxResult.OK)
        {
            return MessageBox.Show(text, "Steam Account Switcher", btn, img, defaultbtn);
        }
    }
}