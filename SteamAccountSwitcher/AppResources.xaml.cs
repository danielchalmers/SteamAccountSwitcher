using System.Windows;

namespace SteamAccountSwitcher
{
    partial class AppResources : ResourceDictionary
    {
        public AppResources()
        {
            InitializeComponent();
        }

        private void TrayIcon_OnTrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            App.ShowAndActivateMainWindow();
        }

        private void MenuItemOptions_OnClick(object sender, RoutedEventArgs e)
        {
            SettingsHelper.OpenOptions();
        }

        private void MenuItemExit_OnClick(object sender, RoutedEventArgs e)
        {
            AppHelper.Shutdown();
        }

        private void MenuItemAddAccount_OnClick(object sender, RoutedEventArgs e)
        {
            AccountHelper.New();
        }

        private void MenuItemManageAccount_OnClick(object sender, RoutedEventArgs e)
        {
            App.ShowAndActivateMainWindow();
        }

        private void MenuItemOpenSteam_OnClick(object sender, RoutedEventArgs e)
        {
            SteamClient.Launch();
        }

        private void MenuItemExitSteam_OnClick(object sender, RoutedEventArgs e)
        {
            SteamClient.Logout();
        }
    }
}