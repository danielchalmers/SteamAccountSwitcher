#region

using System.Collections.Generic;
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
            return SettingsHelper.SerializeAccounts(new List<Account>
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
            App.Accounts = new ObservableCollection<Account>(SettingsHelper.DeserializeAccounts());
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
                Settings.Default.Accounts = dialog.TextData;
                ReloadData();
            }
        }

        public static void ExportAccounts()
        {
            var dialog = new InputBox("Export Accounts", SettingsHelper.SerializeAccounts(App.Accounts));
            dialog.ShowDialog();
        }
    }
}