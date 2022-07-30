using System.Linq;
using System.Windows;
using WpfAboutView;

namespace SteamAccountSwitcher
{
    public partial class AppResources : ResourceDictionary
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

        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            var aboutDialog = Application.Current.Windows.OfType<AboutDialog>().FirstOrDefault() ??
                new AboutDialog
                {
                    AboutView = (AboutView)Application.Current.FindResource("AboutView")
                };

            aboutDialog.ShowDialogOrBringToFront();
        }

        private void MenuItemOptions_Click(object sender, RoutedEventArgs e)
        {
            var optionsDialog = Application.Current.Windows.OfType<Options>().FirstOrDefault() ?? new Options();

            optionsDialog.ShowDialogOrBringToFront();
        }
    }
}