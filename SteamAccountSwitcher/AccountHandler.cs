#region

using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SteamAccountSwitcher;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountswitcher
{
    public class AccountHandler
    {
        public readonly Action _hideWindow;
        public readonly Action _showWindow;
        public readonly Action _refreshWindow;
        private readonly StackPanel _stackPanel;
        private int SelectedIndex = -1;

        public AccountHandler(StackPanel stackPanel, Action hideWindow, Action showWindow, Action refreshWindow)
        {
            _stackPanel = stackPanel;
            _hideWindow = hideWindow;
            _showWindow = showWindow;
            _refreshWindow = refreshWindow;
            Refresh();
        }

        public void Add(Account account)
        {
            App.Accounts.Add(account);
            Refresh();
        }

        public void Refresh()
        {
            Settings.Default.Save();
            // Remove all buttons.
            _stackPanel.Children.Clear();

            // Add new buttons with saved shortcut data
            foreach (var btn in App.Accounts.Select(account => new Button
            {
                Content =
                    new TextBlock
                    {
                        Text = GetAccountDisplayName(account),
                        TextWrapping = TextWrapping.Wrap
                    },
                Height = Settings.Default.ButtonHeight,
                HorizontalContentAlignment = Settings.Default.ButtonTextAlignment.ToHorizontalAlignment(),
                Padding = new Thickness(4),
                ContextMenu = new MenuHelper(this).AccountMenu(),
                Background = new SolidColorBrush(account.Color),
                Foreground = new SolidColorBrush(Settings.Default.ButtonTextColor)
            }))
            {
                btn.Click += Button_Click;

                _stackPanel.Children.Add(btn);
            }

            _refreshWindow();
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
            if (SteamClient.LogOutTimeout())
                SteamClient.LogIn(App.Accounts[SelectedIndex]);
        }

        public void OpenPropeties()
        {
            var dialog = new AccountProperties(App.Accounts[SelectedIndex]);
            dialog.ShowDialog();
            if (dialog.NewAccount == null)
                return;
            App.Accounts[SelectedIndex] = dialog.NewAccount;
            Refresh();
        }

        public void New()
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

        public void MoveTop(int i = -2, bool update = true)
        {
            var index = i == -2 ? SelectedIndex : i;
            var account = App.Accounts[index];
            App.Accounts.Remove(account);
            App.Accounts.Insert(0, account);
            if (update)
                Refresh();
        }

        public void MoveBottom(int i = -2, bool update = true)
        {
            var index = i == -2 ? SelectedIndex : i;
            var account = App.Accounts[index];
            App.Accounts.Remove(account);
            App.Accounts.Insert(App.Accounts.Count, account);
            if (update)
                Refresh();
        }

        public void MoveUp(int i = -2, bool update = true)
        {
            var index = i == -2 ? SelectedIndex : i;
            if (index == 0)
                MoveBottom(index, false);
            else
                App.Accounts.Swap(index, index - 1);
            if (update)
                Refresh();
        }

        public void MoveDown(int i = -2, bool update = true)
        {
            var index = i == -2 ? SelectedIndex : i;
            if (App.Accounts.Count <= index + 1)
                MoveTop(index, false);
            else
                App.Accounts.Swap(index, index + 1);
            if (update)
                Refresh();
        }

        public void Remove(int i = -2, bool msg = false)
        {
            var index = i == -2 ? SelectedIndex : i;
            if (index < 0 || index > App.Accounts.Count - 1) return;
            if (msg &&
                Popup.Show(
                    $"Are you sure you want to delete this account?\r\n\r\n\"{GetAccountDisplayName(App.Accounts[index])}\"",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes) == MessageBoxResult.No)
                return;
            App.Accounts.RemoveAt(index);
            Refresh();
        }

        private static string GetAccountDisplayName(Account account)
        {
            return string.IsNullOrWhiteSpace(account.DisplayName)
                ? account.Username
                : account.DisplayName;
        }
    }
}