using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using SteamAccountSwitcher.Properties;
using WpfWindowPlacement;

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
                App.ShowAndActivateMainWindow();
            }

            return IntPtr.Zero;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Settings.Default.SwitchWindowPlacement = this.GetPlacement();
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

            if (!isNowVisible && wasVisible && Settings.Default.AlwaysOn && !App.IsShuttingDown)
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.SwitchWindowRememberPosition)
                this.SetPlacement(Settings.Default.SwitchWindowPlacement);
        }
    }
}