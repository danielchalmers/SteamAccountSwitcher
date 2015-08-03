#region

using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
            _accountHandler = new AccountHandler(spAccounts, Hide, Show);

            // Assign context menus.
            Menu = new MenuHelper(_accountHandler).MainMenu();
            notifyIcon.ContextMenu = new MenuHelper(_accountHandler).NotifyMenu();

            if (Settings.Default.AlwaysOn)
                Hide();

            if (Settings.Default.OnStartLoginName != "" && Settings.Default.AlwaysOn)
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

            if (spAccounts.Children.Count > 0)
                spAccounts.Children[0].Focus();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (Settings.Default.AlwaysOn)
            {
                e.Cancel = true;
                Hide();
                return;
            }
            App.HelperWindow.Close();
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
    }
}