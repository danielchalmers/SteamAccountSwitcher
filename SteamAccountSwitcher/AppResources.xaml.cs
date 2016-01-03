using System.Windows;

namespace SteamAccountSwitcher
{
    partial class AppResources : ResourceDictionary
    {
        public AppResources()
        {
            InitializeComponent();
        }

        private void menuItemOptions_OnClick(object sender, RoutedEventArgs e)
        {
            SettingsHelper.OpenOptions();
        }

        private void menuItemExit_OnClick(object sender, RoutedEventArgs e)
        {
            AppHelper.ShutdownApplication();
        }

        private void TrayIcon_OnTrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            SwitchWindowHelper.ActivateSwitchWindow();
        }
    }
}