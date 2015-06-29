#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
            // Upgrade settings.
            SettingsHelper.UpgradeSettings();

            // Increment launch times.
            Settings.Default.Launches++;

            // Add default shortcuts.
            if (ClickOnceHelper.IsFirstLaunch && Settings.Default.Accounts == string.Empty)
                Settings.Default.Accounts =
                    SettingsHelper.SerializeAccounts(new List<Account>
                    {
                        new Account
                        {
                            DisplayName = "Example",
                            Username = "username",
                            Password = "password"
                        }
                    });

            // Resolve Steam path.
            if (Settings.Default.SteamPath == string.Empty || !File.Exists(Settings.Default.SteamPath))
                Settings.Default.SteamPath = SteamClient.ResolvePath();
            if (Settings.Default.SteamPath == string.Empty)
            {
                Popup.Show("Steam path could not be located. Application will now exit.");
                Application.Current.Shutdown();
            }

            // Setup account handler.
            _accountHandler = new AccountHandler(spAccounts, Hide, Show);

            // Assign context menus.
            Menu = new MenuHelper(_accountHandler).MainMenu();
            notifyIcon.ContextMenu = new MenuHelper(_accountHandler).NotifyMenu();

            if (Settings.Default.AlwaysOn)
                Hide();

            if (Settings.Default.OnStartLoginName != "" && Settings.Default.AlwaysOn)
            {
                foreach (var account in _accountHandler.Accounts.Where(x => x.Username == Settings.Default.OnStartLoginName))
                {
                    _accountHandler.SwitchAccount(_accountHandler.Accounts.IndexOf(account));
                    break;
                }
            }

            // Start update checker.
            UpdateChecker.Start();

            if (spAccounts.Children.Count > 0)
                spAccounts.Children[0].Focus();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (Settings.Default.AlwaysOn)
            {
                e.Cancel = true;
                Hide();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SettingsHelper.SaveSettings(_accountHandler);
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