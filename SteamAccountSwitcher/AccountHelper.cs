#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    public static class AccountHelper
    {
        private static void Add(Account account)
        {
            account.AddDate = DateTime.Now;
            App.Accounts.Add(account);
        }

        public static void SwitchTo(this Account account, bool hideWindow = true, bool onStart = false)
        {
            if (hideWindow && Settings.Default.ExitOnSwitch)
                App.SwitchWindow.HideWindow();
            var worker = new BackgroundWorker();
            worker.DoWork += delegate
            {
                SteamClient.LogOutTimeout();
                SteamClient.LogIn(account, onStart);
            };
            worker.RunWorkerCompleted += delegate
            {
                if (!Settings.Default.AlwaysOn && Settings.Default.ExitOnSwitch)
                    AppHelper.ShutdownApplication();
            };
            worker.RunWorkerAsync();
        }

        public static void Edit(this Account account)
        {
            var dialog = new AccountProperties(account);
            dialog.ShowDialog();
            var newAccount = dialog.NewAccount;
            if (newAccount == null)
                return;
            newAccount.LastModifiedDate = DateTime.Now;
            App.Accounts[App.Accounts.IndexOf(account)] = newAccount;
        }

        public static void New()
        {
            var dialog = new AccountProperties();
            dialog.ShowDialog();
            if (dialog.NewAccount == null)
                return;
            if (string.IsNullOrWhiteSpace(dialog.NewAccount.Username) &&
                string.IsNullOrWhiteSpace(dialog.NewAccount.Password) &&
                string.IsNullOrWhiteSpace(dialog.NewAccount.DisplayName))
                return;
            Add(dialog.NewAccount);
        }

        public static void Remove(this Account account, bool msg = false)
        {
            if (msg &&
                Popup.Show(
                    $"Are you sure you want to remove \"{account.GetDisplayName()}\"?",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.No)
                return;
            App.Accounts.Remove(account);
        }

        public static void MoveUp(this Account account, bool toEnd = false)
        {
            var index = App.Accounts.IndexOf(account);
            if (toEnd)
            {
                App.Accounts.Swap(index, 0);
            }
            else
            {
                if (index == 0)
                    MoveDown(account, true);
                else
                    App.Accounts.Swap(index, index - 1);
            }
        }

        public static void MoveDown(this Account account, bool toEnd = false)
        {
            var index = App.Accounts.IndexOf(account);
            if (toEnd)
            {
                App.Accounts.Swap(index, App.Accounts.Count - 1);
            }
            else
            {
                if (App.Accounts.Count - 1 < index + 1)
                    MoveUp(account, true);
                else
                    App.Accounts.Swap(index, index + 1);
            }
        }

        public static void Reload(this IEnumerable<Account> accounts)
        {
            App.Accounts = new ObservableCollection<Account>(accounts);
            SettingsHelper.SaveSettings();
            AccountDataHelper.ReloadData();
        }
    }
}