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
            _accountHandler = new AccountHandler(stackPanel, HideUI, UpdateUI);

            // Assign context menu.
            Menu = new MenuHelper(_accountHandler).MainMenu();

            UpdateUI();

            if (stackPanel.Children.Count > 0)
                stackPanel.Children[0].Focus();
        }

        private void HideUI()
        {
            Hide();
            if (!Settings.Default.AlwaysOn)
                notifyIcon.Visibility = Visibility.Hidden;
        }

        private void ShowUI()
        {
            Show();
            if (WindowState == WindowState.Minimized)
                WindowState = WindowState.Normal;
        }

        private void UpdateUI()
        {
            // Replace tool menu with notify icon or vise versa.
            notifyIcon.ContextMenu = new MenuHelper(_accountHandler).NotifyMenu();
            notifyIcon.Visibility = Settings.Default.NotifyIcon ? Visibility.Visible : Visibility.Hidden;
            toolMenu.Visibility = (Settings.Default.NotifyIcon && !Settings.Default.AlwaysShowToolMenu)
                ? Visibility.Hidden
                : Visibility.Visible;

            AutoResize();

            if (Settings.Default.AlwaysOn)
                HideUI();
            else
                ShowUI();
        }

        private double CalcHeight(int count)
        {
            var snugContentHeight = (count*Settings.Default.ButtonHeight) + (count*1);
            var horizontalBorderHeight = SystemParameters.ResizeFrameHorizontalBorderHeight;
            var captionHeight = SystemParameters.CaptionHeight;
            return ((snugContentHeight + captionHeight + 2*horizontalBorderHeight) + 8) +
                   ((Settings.Default.NotifyIcon && !Settings.Default.AlwaysShowToolMenu) ? 0 : toolMenu.Height);
        }

        private double CalcWidth()
        {
            const int snugContentWidth = 400;
            var verticalBorderWidth = SystemParameters.ResizeFrameVerticalBorderWidth;
            return snugContentWidth + 2*verticalBorderWidth;
        }

        private void AutoResize()
        {
            if (_accountHandler?.Accounts == null)
                return;
            Width = CalcWidth();
            Height = CalcHeight(_accountHandler.Accounts.Count) < CalcHeight(3)
                ? CalcHeight(3)
                : CalcHeight(_accountHandler.Accounts.Count);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            SettingsHelper.SaveSettings(_accountHandler);

            if (Settings.Default.AlwaysOn)
            {
                e.Cancel = true;
                HideUI();
            }
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
            ShowUI();
        }
    }
}