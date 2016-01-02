#region

using System;
using System.ComponentModel;
using System.Linq;
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
        public SwitchWindow()
        {
            InitializeComponent();

            if (!Settings.Default.SwitchWindowKeepCentered &&
                (!double.IsNaN(Settings.Default.SwitchWindowLeft) || !double.IsNaN(Settings.Default.SwitchWindowTop)))
            {
                WindowStartupLocation = WindowStartupLocation.Manual;
                Left = Settings.Default.SwitchWindowLeft;
                Top = Settings.Default.SwitchWindowTop;
            }
            else
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            ReloadAccountListBinding();
        }

        public Account SelectedAccount { get; private set; }

        public void ReloadAccountListBinding()
        {
            AccountView.DataContext = null;
            AccountView.DataContext = App.Accounts;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Settings.Default.SwitchWindowLeft = Left;
            Settings.Default.SwitchWindowTop = Top;
            if (App.IsShuttingDown)
                return;
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
            new Options().ShowDialog();
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

        private void SetFocus(object sender)
        {
            SelectedAccount = ((sender as Control)?.Tag as Account);
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var mainWindowPtr = new WindowInteropHelper(this).Handle;
            var mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
            mainWindowSrc?.AddHook(SingleInstanceHelper.WndProc);
        }

        private void btnAccount_OnClick(object sender, RoutedEventArgs e)
        {
            SetFocus(sender);
            SelectedAccount.SwitchTo();
        }

        private void btnAccount_OnPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            SetFocus(sender);
        }

        private void menuItemEdit_OnClick(object sender, RoutedEventArgs e)
        {
            SelectedAccount.Edit();
        }

        private void menuItemMoveUp_OnClick(object sender, RoutedEventArgs e)
        {
            SelectedAccount.MoveUp();
        }

        private void menuItemMoveDown_OnClick(object sender, RoutedEventArgs e)
        {
            SelectedAccount.MoveDown();
        }

        private void menuItemRemove_OnClick(object sender, RoutedEventArgs e)
        {
            SelectedAccount.Remove(true);
        }

        private void menuItemOptions_OnClick(object sender, RoutedEventArgs e)
        {
            SettingsHelper.OpenOptions();
        }
        
        private void menuItemExit_OnClick(object sender, RoutedEventArgs e)
        {
            ClickOnceHelper.ShutdownApplication();
        }

        private void menuItemSortAlias_OnClick(object sender, RoutedEventArgs e)
        {
            App.Accounts.OrderBy(x => x.DisplayName).Reload();
        }

        private void menuItemSortUsername_OnClick(object sender, RoutedEventArgs e)
        {
            App.Accounts.OrderBy(x => x.Username).Reload();
        }

        private void menuItemSortAddDate_OnClick(object sender, RoutedEventArgs e)
        {
            App.Accounts.OrderBy(x => x.AddDate).Reload();
        }

        private void menuItemSortModifiedDate_OnClick(object sender, RoutedEventArgs e)
        {
            App.Accounts.OrderBy(x => x.LastModifiedDate).Reload();
        }

        private void menuItemSortDescendingAlias_OnClick(object sender, RoutedEventArgs e)
        {
            App.Accounts.OrderByDescending(x => x.DisplayName).Reload();
        }

        private void menuItemSortDescendingUsername_OnClick(object sender, RoutedEventArgs e)
        {
            App.Accounts.OrderByDescending(x => x.Username).Reload();
        }

        private void menuItemSortDescendingAddDate_OnClick(object sender, RoutedEventArgs e)
        {
            App.Accounts.OrderByDescending(x => x.AddDate).Reload();
        }

        private void menuItemSortDescendingModifiedDate_OnClick(object sender, RoutedEventArgs e)
        {
            App.Accounts.OrderByDescending(x => x.LastModifiedDate).Reload();
        }

        private void btnAccountHelper_OnClick(object sender, RoutedEventArgs e)
        {
            SetFocus(sender);
            ((Button) sender).ContextMenu.IsOpen = true;
        }
    }
}