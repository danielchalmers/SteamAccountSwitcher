#region

using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Hardcodet.Wpf.TaskbarNotification;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AccountHandler _accountHandler;
        public readonly ContextMenu Menu;

        public MainWindow()
        {
            InitializeComponent();

            // Setup account handler.
            _accountHandler = new AccountHandler(spAccounts, HideWindow, Show, Refresh);

            // Assign context menus.
            Menu = new MenuHelper(_accountHandler).MainMenu();

            if (!string.IsNullOrWhiteSpace(Settings.Default.OnStartLoginName) && Settings.Default.AlwaysOn &&
                App.Arguments.Contains("-systemstartup"))
            {
                foreach (
                    var account in
                        App.Accounts.Where(x => x.Username == Settings.Default.OnStartLoginName)
                    )
                {
                    _accountHandler.SwitchAccount(App.Accounts.IndexOf(account), true);
                    break;
                }
            }

            Refresh();

            if (spAccounts.Children.Count > 0)
                spAccounts.Children[0].Focus();
        }

        public void Refresh()
        {
            if (Settings.Default.AlwaysOn)
                notifyIcon.ContextMenu = new MenuHelper(_accountHandler).NotifyMenu();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (Settings.Default.AlwaysOn)
            {
                e.Cancel = true;
                HideWindow();
                return;
            }
            ClickOnceHelper.ShutdownApplication();
        }

        private void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            Menu.IsOpen = true;
        }

        private void btnAddAccount_Click(object sender, RoutedEventArgs e)
        {
            _accountHandler.New();
        }

        private void notifyIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            Show();
        }

        private void HideWindow()
        {
            var visible = Visibility == Visibility.Visible;
            Hide();
            if (visible && Settings.Default.AlwaysOn)
                ShowRunningInTrayBalloon();
        }

        public void ShowRunningInTrayBalloon()
        {
            ShowTrayBalloon(
                $"{Properties.Resources.AppName} is running in system tray.\nDouble click icon to show window.",
                BalloonIcon.Info);
        }

        public void ShowTrayBalloon(string text, BalloonIcon icon)
        {
            if (!Settings.Default.ShowTrayNotifications)
                return;
            notifyIcon.ShowBalloonTip(Properties.Resources.AppName, text, icon);
        }

        private void MainWindow_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!Settings.Default.NumberHotkeys)
                return;
            var num = 0;
            switch (e.Key)
            {
                case Key.D1:
                    num = 1;
                    break;
                case Key.D2:
                    num = 2;
                    break;
                case Key.D3:
                    num = 3;
                    break;
                case Key.D4:
                    num = 4;
                    break;
                case Key.D5:
                    num = 5;
                    break;
                case Key.D6:
                    num = 6;
                    break;
                case Key.D7:
                    num = 7;
                    break;
                case Key.D8:
                    num = 8;
                    break;
                case Key.D9:
                    num = 9;
                    break;
                case Key.D0:
                    num = 10;
                    break;
            }
            if (num > 0 && spAccounts.Children.Count >= num)
                _accountHandler.SwitchAccount(num - 1);
        }
    }
}