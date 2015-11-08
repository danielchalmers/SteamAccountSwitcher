#region

using System;
using System.Collections.ObjectModel;
using System.Windows;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    internal class AccountDataHelper
    {
        public static string DefaultData()
        {
            return SettingsHelper.SerializeAccounts(new ObservableCollection<Account>
            {
                new Account
                {
                    DisplayName = "Example",
                    Username = "username",
                    Password = "password"
                }
            });
        }

        public static void ReloadData()
        {
            try
            {
                App.Accounts =
                    new ObservableCollection<Account>(SettingsHelper.DeserializeAccounts(Settings.Default.Accounts));
            }
            catch
            {
                App.Accounts = new ObservableCollection<Account>();
                Popup.Show(
                    $"Existing account data is corrupt.{Environment.NewLine}{Environment.NewLine}All accounts have been reset.",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            App.Accounts.CollectionChanged += (sender, args) => App.SaveTimer.DelaySave();
            App.SwitchWindow?.ReloadAccountListBinding();
        }

        public static void ImportAccounts()
        {
            var dialog = new InputBox("Import Accounts");
            dialog.ShowDialog();
            if (dialog.Cancelled == false &&
                Popup.Show(
                    "Are you sure you want to overwrite all current accounts?\n\nThis cannot be undone.",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                try
                {
                    // Test import data before overwriting existing accounts.
                    var testAccounts =
                        new ObservableCollection<Account>(SettingsHelper.DeserializeAccounts(dialog.TextData));

                    Settings.Default.Accounts = dialog.TextData;
                    ReloadData();
                    SettingsHelper.SaveSettings();
                }
                catch
                {
                    Popup.Show(
                        $"Import failed. Data may be corrupt.{Environment.NewLine}{Environment.NewLine}No changes have been made.",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        public static void ExportAccounts()
        {
            var dialog = new InputBox("Export Accounts", SettingsHelper.SerializeAccounts(App.Accounts));
            dialog.ShowDialog();
        }
    }
}