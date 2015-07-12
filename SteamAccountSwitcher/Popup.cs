#region

using System.Windows;
using System.Windows.Media;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    internal class Popup
    {
        public static MessageBoxResult Show(string text, MessageBoxButton btn = MessageBoxButton.OK,
            MessageBoxImage img = MessageBoxImage.Information, MessageBoxResult defaultbtn = MessageBoxResult.OK)
        {
            var window = new Window
            {
                Visibility = Visibility.Hidden,
                AllowsTransparency = true,
                Background = Brushes.Transparent,
                WindowStyle = WindowStyle.None,
                ShowInTaskbar = false
            };

            window.Show();
            var msg = MessageBox.Show(window, text, Resources.AppName, btn, img, defaultbtn);
            window.Close();
            return msg;
        }
    }
}