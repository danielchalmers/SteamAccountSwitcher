#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    public class AccountHandler
    {
        public readonly Action _hideWindow;
        public readonly Action _showWindow;
        private readonly StackPanel _stackPanel;
        private readonly Action _updateUI;
        public readonly List<Account> Accounts;
        private int SelectedIndex = -1;

        public AccountHandler(StackPanel stackPanel, Action hideWindow, Action showWindow, Action updateUI)
        {
            _stackPanel = stackPanel;
            Accounts = SettingsHelper.DeserializeAccounts() ?? new List<Account>();
            _hideWindow = hideWindow;
            _showWindow = showWindow;
            _updateUI = updateUI;
            Refresh();
        }

        public void Add(Account account)
        {
            Accounts.Add(account);
            Refresh();
        }

        public void Refresh()
        {
            // Remove all buttons.
            _stackPanel.Children.Clear();

            // Add new buttons with saved shortcut data
            foreach (var btn in Accounts.Select(account => new Button
            {
                Content =
                    new TextBlock
                    {
                        Text = GetAccountDisplayName(account),
                        TextWrapping = TextWrapping.Wrap
                    },
                Height = Settings.Default.ButtonHeight,
                HorizontalContentAlignment = Settings.Default.ButtonTextAlignment,
                Padding = new Thickness(4, 0, 4, 0),
                ContextMenu = new MenuHelper(this).AccountMenu(),
                Background = account.Color ?? new SolidColorBrush(Settings.Default.ButtonDefaultColor),
                Foreground = new SolidColorBrush(Settings.Default.ButtonTextColor)
            }))
            {
                btn.Click += Button_Click;

                _stackPanel.Children.Add(btn);
            }
            _updateUI();
        }

        private void SetFocus(object sender)
        {
            SelectedIndex = _stackPanel.Children.IndexOf((Button) sender);
        }

        public void SetFocus(object sender, RoutedEventArgs e)
        {
            SetFocus(((ContextMenu) sender).PlacementTarget);
        }

        public void SwitchAccount(int index)
        {
            SelectedIndex = index;
            var worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_Completed;
            worker.RunWorkerAsync();
            _hideWindow();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SetFocus(sender);
            SwitchAccount(SelectedIndex);
        }

        private void worker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!Settings.Default.AlwaysOn)
                Application.Current.Shutdown();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (SteamClient.LogOutAuto())
                SteamClient.LogIn(Accounts[SelectedIndex]);
        }

        public void OpenPropeties()
        {
            var dialog = new AccountProperties(Accounts[SelectedIndex]);
            dialog.ShowDialog();
            if (!string.IsNullOrWhiteSpace(dialog.NewAccount.Username))
                Accounts[SelectedIndex] = dialog.NewAccount;
            Refresh();
        }

        public void New()
        {
            var dialog = new AccountProperties();
            dialog.ShowDialog();
            if (string.IsNullOrWhiteSpace(dialog.NewAccount.Username) &&
                string.IsNullOrWhiteSpace(dialog.NewAccount.Password) &&
                string.IsNullOrWhiteSpace(dialog.NewAccount.DisplayName))
                return;
            Add(dialog.NewAccount);
        }

        public void MoveUp(int i = -2)
        {
            var index = i == -2 ? SelectedIndex : i;
            if (index == 0) return;
            Accounts.Swap(index, index - 1);
            Refresh();
        }

        public void MoveDown(int i = -2)
        {
            var index = i == -2 ? SelectedIndex : i;
            if (Accounts.Count <= index + 1) return;
            Accounts.Swap(index, index + 1);
            Refresh();
        }

        public void Remove(int i = -2, bool msg = false)
        {
            var index = i == -2 ? SelectedIndex : i;
            if (index < 0 || index > Accounts.Count - 1) return;
            if (msg &&
                Popup.Show(
                    $"Are you sure you want to delete this account?\r\n\r\n\"{GetAccountDisplayName(Accounts[index])}\"",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes) == MessageBoxResult.No)
                return;
            Accounts.RemoveAt(index);
            Refresh();
        }

        private static string GetAccountDisplayName(Account account)
        {
            return string.IsNullOrWhiteSpace(account.Username)
                ? account.DisplayName
                : account.Username;
        }
    }
}