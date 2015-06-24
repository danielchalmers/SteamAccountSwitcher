#region

using System.Collections.Generic;
using System.ComponentModel;
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
            if (Settings.Default.SteamPath == string.Empty)
                Settings.Default.SteamPath = SteamClient.ResolvePath();
            if (Settings.Default.SteamPath == string.Empty)
            {
                Popup.Show("Steam path could not be located. Application will now exit.");
                Application.Current.Shutdown();
            }

            // Setup account handler.
            _accountHandler = new AccountHandler(stackPanel, Hide, Show, UpdateUI);

            // Assign context menus.
            Menu = new MenuHelper(_accountHandler).MainMenu();
            notifyIcon.ContextMenu = new MenuHelper(_accountHandler).NotifyMenu();

            // Start update checker.
            UpdateChecker.Start();

            UpdateUI();

            if (stackPanel.Children.Count > 0)
                stackPanel.Children[0].Focus();
        }

        private void UpdateUI()
        {
            // Replace tool menu with notify icon or vise versa.
            notifyIcon.Visibility = Settings.Default.AlwaysOn ? Visibility.Visible : Visibility.Hidden;
            toolMenu.Visibility = Settings.Default.AlwaysOn ? Visibility.Collapsed : Visibility.Visible;

            if (Settings.Default.AlwaysOn)
                Hide();
            else
                Show();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (Settings.Default.AlwaysOn)
            {
                e.Cancel = true;
                Hide();
                return;
            }

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