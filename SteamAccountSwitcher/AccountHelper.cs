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
            {
                SwitchWindowHelper.HideSwitcherWindow();
            }
            var worker = new BackgroundWorker();
            worker.DoWork += delegate
            {
                SteamClient.LogOutTimeout();
                SteamClient.LogIn(account, onStart);
            };
            worker.RunWorkerCompleted += delegate
            {
                if (!Settings.Default.AlwaysOn && Settings.Default.ExitOnSwitch)
                {
                    AppHelper.ShutdownApplication();
                }
            };
            worker.RunWorkerAsync();
        }

        public static void Edit(this Account account)
        {
            var dialog = new AccountProperties(account);
            dialog.ShowDialog();
            if (dialog.DialogResult != true)
            {
                return;
            }
            var newAccount = dialog.NewAccount;
            if (newAccount == null)
            {
                return;
            }
            newAccount.LastModifiedDate = DateTime.Now;
            App.Accounts[App.Accounts.IndexOf(account)] = newAccount;
            App.SaveTimer.DelaySave();
        }

        public static void New()
        {
            var dialog = new AccountProperties();
            dialog.ShowDialog();
            if (dialog.DialogResult != true)
            {
                return;
            }
            if (dialog.NewAccount == null)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(dialog.NewAccount.Username) &&
                string.IsNullOrWhiteSpace(dialog.NewAccount.Password) &&
                string.IsNullOrWhiteSpace(dialog.NewAccount.DisplayName))
            {
                return;
            }
            Add(dialog.NewAccount);
        }

        public static void Remove(this Account account, bool msg = false)
        {
            if (msg &&
                Popup.Show(
                    $"Are you sure you want to remove \"{account.GetDisplayName()}\"?",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.No)
            {
                return;
            }
            App.Accounts.Remove(account);
        }

        public static Account MoveUp(this Account account)
        {
            return App.Accounts.MoveUp(account);
        }

        public static Account MoveDown(this Account account)
        {
            return App.Accounts.MoveDown(account);
        }

        public static void Reload(this IEnumerable<Account> accounts)
        {
            App.Accounts = new ObservableCollection<Account>(accounts);
            SettingsHelper.SaveSettings();
            AccountDataHelper.ReloadData();
        }
    }
}