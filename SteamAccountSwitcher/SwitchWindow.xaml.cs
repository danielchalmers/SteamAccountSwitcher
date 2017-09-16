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
                : WindowStartupLocation.CenterScreen
;
            ReloadAccountListBinding();
        }

        public void ReloadAccountListBinding()
        {
            AccountView.DataContext = null;
            AccountView.DataContext = App.Accounts;
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Settings.Default.SwitchWindowPlacement) &&
                Settings.Default.SwitchWindowRememberPosition)
            {
                this.SetPlacement(Settings.Default.SwitchWindowPlacement);
            }
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
                HideWindow();
                return;
            }
            Settings.Default.SwitchWindowPlacement = this.GetPlacement();
            AppHelper.ShutdownApplication();
        }

        public void HideWindow()
        {
            var visible = Visibility == Visibility.Visible;
            Hide();
            if (visible && Settings.Default.AlwaysOn)
            {
                TrayIconHelper.ShowRunningInTrayBalloon();
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var mainWindowPtr = new WindowInteropHelper(this).Handle;
            var mainWindowSrc = HwndSource.FromHwnd(mainWindowPtr);
            mainWindowSrc?.AddHook(WndProc);
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
                        SwitchWindowHelper.ActivateSwitchWindow();
                    }
                }
                else
                {
                    SwitchWindowHelper.ActivateSwitchWindow();
                }
            }

            return IntPtr.Zero;
        }
    }
}