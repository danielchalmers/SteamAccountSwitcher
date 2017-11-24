using System;
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
            Owner = Application.Current.MainWindow;
            Settings.Default.Save();
        }

        private void menuItemImport_OnClick(object sender, EventArgs e)
        {
            AccountDataHelper.ImportAccounts();
        }

        private void menuItemExport_OnClick(object sender, EventArgs e)
        {
            AccountDataHelper.ExportAccounts();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (DialogResult == true)
            {
                SettingsHelper.SaveSettings();
            }
            else
            {
                Settings.Default.Reload();
            }
        }
    }
}