using System;
using System.Diagnostics;
using System.Windows;
using SteamAccountSwitcher.Properties;

namespace SteamAccountSwitcher
{
    /// <summary>
    ///     Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        public Options()
        {
            InitializeComponent();
            Settings.Default.Save();
        }

        private void GitHubHyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Properties.Resources.GitHub);
        }

        private void LicensesHyperlink_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(Properties.Resources.Licenses);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (DialogResult == true)
            {
                Settings.Default.Save();
            }
            else
            {
                Settings.Default.Reload();
            }
        }
    }
}