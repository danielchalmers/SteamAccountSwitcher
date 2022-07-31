using System.Linq;
using System.Windows;
using System.Windows.Controls;
using H.NotifyIcon.Core;
using WpfAboutView;

namespace SteamAccountSwitcher
{
    public partial class AppResources : ResourceDictionary
    {
        public AppResources()
        {
            InitializeComponent();
        }

        private async void MenuItemAccount_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            var account = (SteamAccount)menuItem.Tag;

            await SteamClient.Exit();

            if (!SteamClient.TrySetLoginUser(account.Name))
            {
                App.TrayIcon.ShowNotification(
                    "Couldn't switch account automatically",
                    "We might not have permission to change the registry",
                    NotificationIcon.Error);
            }

            SteamClient.Launch();
        }

        private async void MenuItemAddAccount_Click(object sender, RoutedEventArgs e)
        {
            await SteamClient.Exit();

            if (!SteamClient.TryResetLoginUser())
            {
                App.TrayIcon.ShowNotification(
                    "Couldn't log out automatically",
                    "Please log of out Steam and try again",
                    NotificationIcon.Error);
            }

            SteamClient.Launch();
        }

        private void MenuItemOptions_Click(object sender, RoutedEventArgs e)
        {
            var optionsDialog = Application.Current.Windows.OfType<Options>().FirstOrDefault() ?? new Options();

            if (optionsDialog.IsVisible)
            {
                optionsDialog.Activate();
                return;
            }

            optionsDialog.ShowDialog();
        }

        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            var aboutDialog = Application.Current.Windows.OfType<AboutDialog>().FirstOrDefault() ??
                new AboutDialog
                {
                    AboutView = (AboutView)Application.Current.FindResource("AboutView"),
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

            if (aboutDialog.IsVisible)
            {
                aboutDialog.Activate();
                return;
            }

            aboutDialog.ShowDialog();
        }

        private void MenuItemExit_OnClick(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}