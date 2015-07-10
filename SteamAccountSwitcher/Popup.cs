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
            var window = new Window
            {
                Visibility = Visibility.Hidden,
                AllowsTransparency = true,
                Background = System.Windows.Media.Brushes.Transparent,
                WindowStyle = WindowStyle.None,
                ShowInTaskbar = false
            };

            window.Show();
            var msg = MessageBox.Show(window, text, Properties.Resources.AppName, btn, img, defaultbtn);
            window.Close();
            return msg;
        }
    }
}