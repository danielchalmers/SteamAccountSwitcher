#region

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class SwitchWindow : Window
    {
        public readonly ContextMenu AccountMenu;
        public readonly ContextMenu Menu;

        public SwitchWindow()
        {
            InitializeComponent();

            // Assign context menus.
            Menu = MenuHelper.MainMenu();
            AccountMenu = MenuHelper.AccountMenu();

            AccountView.DataContext = App.Accounts;
        }

        public Account SelectedAccount { get; private set; }

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
            AccountHelper.New();
        }

        public void HideWindow()
        {
            var visible = Visibility == Visibility.Visible;
            Hide();
            if (visible && Settings.Default.AlwaysOn)
                TrayIconHelper.ShowRunningInTrayBalloon();
        }

        private void MainWindow_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!Settings.Default.NumberHotkeys)
                return;

            var num = KeyHelper.KeyToInt(e.Key);
            if (num > 0 && App.Accounts.Count >= num)
                App.Accounts[num - 1].SwitchTo();
        }

        private void btnAccount_Click(object sender, RoutedEventArgs e)
        {
            SelectedAccount.SwitchTo();
        }

        private void btnAccount_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            SelectedAccount = ((sender as Control)?.Tag as Account);
        }

        private void btnAccount_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Right)
                AccountMenu.IsOpen = true;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var mainWindowPtr = new WindowInteropHelper(this).Handle;
            var mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
            mainWindowSrc?.AddHook(SingleInstanceHelper.WndProc);
        }
    }
}