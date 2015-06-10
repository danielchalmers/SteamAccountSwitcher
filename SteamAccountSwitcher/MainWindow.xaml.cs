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


            // Add default shortcuts.
            if (Settings.Default.Accounts == string.Empty && ClickOnceHelper.IsFirstRun)
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

            // Setup account handler.
            _accountHandler = new AccountHandler(stackPanel, Hide, AutoResize);

            if (stackPanel.Children.Count > 0)
                stackPanel.Children[0].Focus();

            AutoResize();

            // Assign context menu.
            Menu = new MenuHelper(_accountHandler).MainMenu();

            // Resolve Steam path.
            if (Settings.Default.SteamPath == string.Empty)
                Settings.Default.SteamPath = SteamClient.ResolvePath();
            if (Settings.Default.SteamPath == string.Empty)
            {
                Popup.Show("Steam path could not be located. Application will now exit.");
                Application.Current.Shutdown();
            }
        }

        private void AutoResize()
        {
            if (_accountHandler?.Accounts == null)
                return;
            const int snugContentWidth = 400;
            var count = _accountHandler.Accounts.Count == 0 ? 5 : _accountHandler.Accounts.Count;
            var snugContentHeight = (count*Settings.Default.ButtonHeight) + (count*1);
            var horizontalBorderHeight = SystemParameters.ResizeFrameHorizontalBorderHeight;
            var verticalBorderWidth = SystemParameters.ResizeFrameVerticalBorderWidth;
            var captionHeight = SystemParameters.CaptionHeight;

            Width = snugContentWidth + 2*verticalBorderWidth;
            Height = ((snugContentHeight + captionHeight + 2*horizontalBorderHeight) + 8) + toolMenu.Height;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            SaveSettings();
        }

        public void SaveSettings()
        {
            // Save settings.
            Settings.Default.Accounts = SettingsHelper.SerializeAccounts(_accountHandler.Accounts);
            Settings.Default.Save();
        }

        private void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            Menu.IsOpen = true;
        }

        private void btnAddAccount_Click(object sender, RoutedEventArgs e)
        {
            _accountHandler.New();
        }
    }
}