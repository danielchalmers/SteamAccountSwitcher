using System.Windows;

namespace SteamAccountSwitcher
{
    partial class AppResources : ResourceDictionary
    {
        public AppResources()
        {
            InitializeComponent();
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