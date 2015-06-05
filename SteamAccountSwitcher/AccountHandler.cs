#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    public class AccountHandler
    {
        private readonly Action _closeWindow;
        private readonly StackPanel _stackPanel;
        public readonly List<Account> Accounts;
        private int SelectedIndex = -1;

        public AccountHandler(StackPanel stackPanel, Action closeWindow)
        {
            _stackPanel = stackPanel;
            Accounts = SettingsHelper.DeserializeAccounts() ?? new List<Account>();
            _closeWindow = closeWindow;
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
                        Text = string.IsNullOrWhiteSpace(account.DisplayName) ? account.Username : account.DisplayName,
                        TextWrapping = TextWrapping.Wrap
                    },
                Height = Settings.Default.ButtonHeight,
                HorizontalContentAlignment = Settings.Default.ButtonTextAlignment,
                Margin = Settings.Default.ButtonMargin,
                Padding = Settings.Default.ButtonPadding,
                ContextMenu = new MenuHelper(this).AccountMenu()
            }))
            {
                btn.Click += Button_Click;

                _stackPanel.Children.Add(btn);
            }
        }

        private void SetFocus(object sender)
        {
            SelectedIndex = _stackPanel.Children.IndexOf((Button) sender);
        }

        public void SetFocus(object sender, RoutedEventArgs e)
        {
            SetFocus(((ContextMenu) sender).PlacementTarget);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SetFocus(sender);
            var worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_Completed;
            worker.RunWorkerAsync();
            _closeWindow();
        }

        private void worker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var processes = Process.GetProcessesByName("steam");
            if (processes.Length > 0)
            {
                SteamClient.LogOut();
                Thread.Sleep(Settings.Default.SwitchWaitTime);
            }
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
            if (!string.IsNullOrWhiteSpace(dialog.NewAccount.Username))
                Add(dialog.NewAccount);
            Refresh();
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
            var accName = string.IsNullOrWhiteSpace(Accounts[index].Username)
                ? Accounts[index].DisplayName
                : Accounts[index].Username;
            if (msg &&
                Popup.Show($"Are you sure you want to delete this account?\r\n\r\n\"{accName}\"",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes) == MessageBoxResult.No)
                return;
            Accounts.RemoveAt(index);
            Refresh();
        }
    }
}