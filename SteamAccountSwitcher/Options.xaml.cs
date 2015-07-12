#region

using System;
using System.Windows;
using SteamAccountswitcher;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    /// <summary>
    ///     Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {
        private readonly AccountHandler _accountHandler;

        public Options(AccountHandler accountHandler)
        {
            InitializeComponent();
            _accountHandler = accountHandler;
            Settings.Default.Save();
            lblVersion.Text = $"v{AssemblyInfo.GetVersionStringFull()}";
            lblVersion.ToolTip = lblVersion.Text;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Save();
            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.Reload();
            DialogResult = true;
        }

        private void btnExtra_Click(object sender, RoutedEventArgs e)
        {
            btnExtra.ContextMenu.IsOpen = true;
        }

        private void menuItemDefaults_OnClick(object sender, EventArgs e)
        {
            if (
                Popup.Show(
                    "Are you sure you want to reset ALL settings (including accounts)?\n\nThis cannot be undone.",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                Settings.Default.Reset();
                AccountDataHelper.ReloadData();
                Popup.Show("All settings have been restored to default.\n\nYou may need to restart this application.");
            }
        }

        private void menuItemImport_OnClick(object sender, EventArgs e)
        {
            var dialog = new InputBox();
            dialog.ShowDialog();
            if (
                Popup.Show(
                    "Are you sure you want to overwrite all current accounts?\n\nThis cannot be undone.",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.No)
                return;
            Settings.Default.Accounts = dialog.TextData;
            AccountDataHelper.ReloadData();
        }

        private void menuItemExport_OnClick(object sender, EventArgs e)
        {
            var dialog = new InputBox(SettingsHelper.SerializeAccounts(App.Accounts));
            dialog.ShowDialog();
        }

        private void menuItemAdvanced_OnClick(object sender, EventArgs e)
        {
            var dialog = new AdvancedOptions();
            dialog.ShowDialog();
        }
    }
}