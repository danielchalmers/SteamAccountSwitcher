#region

using System.Windows;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    internal class Popup
    {
        public static MessageBoxResult Show(string text, MessageBoxButton btn = MessageBoxButton.OK,
            MessageBoxImage img = MessageBoxImage.Information, MessageBoxResult defaultbtn = MessageBoxResult.OK)
        {
            if (Settings.Default.DontShowPopups)
                return MessageBoxResult.Yes;
            var msg = MessageBox.Show(App.SwitchWindow, text, Resources.AppName, btn, img, defaultbtn);
            return msg;
        }
    }
}