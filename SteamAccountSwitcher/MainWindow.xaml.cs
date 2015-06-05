#region

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
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
                            DisplayName = "Example"
                        }
                    });

            // Setup account handler.
            _accountHandler = new AccountHandler(stackPanel, Hide);

            if (stackPanel.Children.Count > 0)
                stackPanel.Children[0].Focus();

            UpdateUI();

            // Add right click context menu.
            ContextMenu = new MenuHelper().MainMenu(_accountHandler, this);

            // Auto resize
            if (Settings.Default.AutoResizeOnStart)
                AutoResize();

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
            const int snugContentWidth = 350;
            var snugContentHeight = (_accountHandler.Accounts.Count*Settings.Default.ButtonHeight) +
                                    ((_accountHandler.Accounts.Count - 1)*Settings.Default.ButtonMargin.Top) +
                                    (_accountHandler.Accounts.Count*Settings.Default.ButtonMargin.Bottom);
            var horizontalBorderHeight = SystemParameters.ResizeFrameHorizontalBorderHeight;
            var verticalBorderWidth = SystemParameters.ResizeFrameVerticalBorderWidth;
            var captionHeight = SystemParameters.CaptionHeight;

            Width = snugContentWidth + 2*verticalBorderWidth;
            Height = (snugContentHeight + captionHeight + 2*horizontalBorderHeight) + 8;

            btnNewAccount.Visibility = Visibility.Hidden;
            btnAddAccount.Visibility = Visibility.Hidden;
            btnOptions.Visibility = Visibility.Hidden;
        }

        public void UpdateUI()
        {
            // Restore window size.
            if (Settings.Default.Height <= MinHeight)
                Settings.Default.Height = 285;
            if (Settings.Default.Width <= MinWidth)
                Settings.Default.Width = 350;
            Height = Settings.Default.Height;
            Width = Settings.Default.Width;

            // Show "New Account" button if enabled in options.
            btnNewAccount.Visibility = Settings.Default.ShowNewAccountButton ? Visibility.Visible : Visibility.Collapsed;
            // Show "Add Account" button if enabled in options.
            btnAddAccount.Visibility = Settings.Default.ShowAddAccountButton ? Visibility.Visible : Visibility.Collapsed;
            // Show "Options" button if enabled in options.
            btnOptions.Visibility = Settings.Default.ShowOptionsButton ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            SaveSettings();
        }

        public void SaveSettings()
        {
            // Save settings.
            Settings.Default.Height = Height;
            Settings.Default.Width = Width;
            Settings.Default.Accounts = SettingsHelper.SerializeAccounts(_accountHandler.Accounts);
            Settings.Default.Save();
        }

        private void btnAddAccount_Click(object sender, RoutedEventArgs e)
        {
            _accountHandler.New();
        }

        private void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            SettingsHelper.OpenOptions(_accountHandler, this);
        }

        private void btnNewAccount_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://store.steampowered.com/join/");
        }
    }
}