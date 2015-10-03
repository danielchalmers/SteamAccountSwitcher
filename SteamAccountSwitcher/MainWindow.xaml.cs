#region

using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;
using SteamAccountswitcher;
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

            if (Settings.Default.AlwaysOn)
                Hide();

            if (Settings.Default.OnStartLoginName != "" && Settings.Default.AlwaysOn && App.Arguments.Contains("-systemstartup"))
            {
                foreach (
                    var account in
                        App.Accounts.Where(x => x.Username == Settings.Default.OnStartLoginName)
                    )
                {
                    _accountHandler.SwitchAccount(App.Accounts.IndexOf(account));
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
            notifyIcon.ShowBalloonTip(Properties.Resources.AppName, $"{Properties.Resources.AppName} is running in system tray.\nDouble click icon to show window.", BalloonIcon.Info);
        }
    }
}