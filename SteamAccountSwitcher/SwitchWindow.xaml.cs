#region

using System;
using System.ComponentModel;
using System.Windows;
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
            ReloadAccountListBinding();
        }

        public void ReloadAccountListBinding()
        {
            AccountView.DataContext = null;
            AccountView.DataContext = App.Accounts;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (App.IsShuttingDown)
                return;
            if (Settings.Default.AlwaysOn)
            {
                e.Cancel = true;
                HideWindow();
                return;
            }
            AppHelper.ShutdownApplication();
        }

        public void HideWindow()
        {
            var visible = Visibility == Visibility.Visible;
            Hide();
            if (visible && Settings.Default.AlwaysOn)
                TrayIconHelper.ShowRunningInTrayBalloon();
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