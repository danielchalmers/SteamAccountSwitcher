using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using SteamAccountSwitcher.Properties;

namespace SteamAccountSwitcher
{
    public static class AccountDataHelper
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
            try
            {
                App.Accounts =
                    new ObservableCollection<Account>(SettingsHelper.DeserializeAccounts(Settings.Default.Accounts));
            }
            catch
            {
                App.Accounts = new ObservableCollection<Account>();
                Popup.Show(
                    "Existing account data is corrupt.\n\n" +
                    "All accounts have been reset.",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            App.Accounts.CollectionChanged += (sender, args) =>
            {
                TrayIconHelper.RefreshTrayIconMenu();
            };
            TrayIconHelper.RefreshTrayIconMenu();
            App.SwitchWindow?.ReloadAccountListBinding();
        }

        public static void ImportAccounts()
        {
            var dialog = new OpenFileDialog
            {
                DefaultExt = Resources.ImportExportExtension,
                Filter = Resources.ImportExportDialogExtensionFilter,
                CheckFileExists = true
            };
            if (dialog.ShowDialog() != true)
            {
                return;
            }
            var fileContent = File.ReadAllText(dialog.FileName);
            try
            {
                // Test imported data before overwriting existing accounts.
                var testAccounts =
                    new ObservableCollection<Account>(SettingsHelper.DeserializeAccounts(fileContent));
            }
            catch
            {
                Popup.Show(
                    "Import failed. Data may be corrupt.\n\n" +
                    "No changes have been made.",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (App.Accounts.Any() &&
                Popup.Show(
                "Are you sure you want to overwrite all current accounts?\n\n" +
                "This cannot be reversed.",
                MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes) != MessageBoxResult.Yes)
            {
                return;
            }
            Settings.Default.Accounts = fileContent;
            ReloadData();
            SettingsHelper.SaveSettings();
        }

        public static void ExportAccounts()
        {
            var dialog = new SaveFileDialog
            {
                DefaultExt = Resources.ImportExportExtension,
                Filter = Resources.ImportExportDialogExtensionFilter,
                FileName = $"{AssemblyInfo.Title} export {DateTime.Now.ToString("yyMMddHHmmss")}",
            };
            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, SettingsHelper.SerializeAccounts(App.Accounts));
            }
        }
    }
}