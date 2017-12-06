using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using SteamAccountSwitcher.Properties;

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
            WindowStartupLocation = Settings.Default.SwitchWindowRememberPosition
                ? WindowStartupLocation.Manual
                : WindowStartupLocation.CenterScreen;
            ReloadAccountListBinding();
        }

        public void ReloadAccountListBinding()
        {
            AccountView.DataContext = null;
            AccountView.DataContext = App.Accounts;
        }

        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeMethods.WM_ACTIVATE)
            {
                if (Settings.Default.AlwaysOn)
                {
                    if (App.SwitchWindow == null || App.SwitchWindow.Visibility != Visibility.Visible)
                    {
                        TrayIconHelper.ShowRunningInTrayBalloon();
                    }
                    else
                    {
                        App.ShowAndActivateMainWindow();
                    }
                }
                else
                {
                    App.ShowAndActivateMainWindow();
                }
            }

            return IntPtr.Zero;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (App.IsShuttingDown)
            {
                return;
            }
            if (Settings.Default.AlwaysOn)
            {
                e.Cancel = true;
                Hide();
                return;
            }
            AppHelper.Shutdown();
        }

        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var wasVisible = (bool)e.OldValue;
            var isNowVisible = (bool)e.NewValue;

            if (!isNowVisible && wasVisible && Settings.Default.AlwaysOn)
            {
                TrayIconHelper.ShowRunningInTrayBalloon();
            }
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            var windowHandle = new WindowInteropHelper(this).Handle;
            var windowSource = HwndSource.FromHwnd(windowHandle);
            windowSource?.AddHook(WndProc);
        }
    }
}