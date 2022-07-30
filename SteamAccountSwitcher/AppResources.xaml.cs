﻿using System.Linq;
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
    }
}