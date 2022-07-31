using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfAboutView;

namespace SteamAccountSwitcher
{
    public partial class AppResources : ResourceDictionary
    {
        public AppResources()
        {
            InitializeComponent();
        }

        private async void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;

            if (menuItem.CommandParameter is string stringParameter)
            {
                if (stringParameter == "add-account")
                {
                    await SteamClient.LogOut();
                }
                else if (stringParameter == "options")
                {
                    var optionsDialog = Application.Current.Windows.OfType<Options>().FirstOrDefault() ?? new Options();

                    if (optionsDialog.IsVisible)
                    {
                        optionsDialog.Activate();
                        return;
                    }

                    optionsDialog.ShowDialog();
                }
                else if (stringParameter == "about")
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
                else if (stringParameter == "exit")
                {
                    App.Current.Shutdown();
                }
            }
            else if (menuItem.CommandParameter is SteamAccount account)
            {
                menuItem.IsEnabled = false;
                await SteamClient.LogIn(account);
                menuItem.IsEnabled = true;
            }
            else
            {
                throw new NotSupportedException();
            }
        }
    }
}