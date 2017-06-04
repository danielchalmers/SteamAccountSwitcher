#region

using System;
using System.Windows;
using SteamAccountSwitcher.Properties;

#endregion

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
            SettingsHelper.SaveSettings();
            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Reload();
            DialogResult = true;
        }
    }
}