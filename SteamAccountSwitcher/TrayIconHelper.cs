using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Hardcodet.Wpf.TaskbarNotification;
using SteamAccountSwitcher.Properties;

namespace SteamAccountSwitcher
{
    internal class TrayIconHelper
    {
        public static void ShowRunningInTrayBalloon()
        {
            ShowTrayBalloon(
                $"{Resources.AppName} is running in system tray.\nDouble click icon to show window.",
                BalloonIcon.Info);
        }

        public static void ShowTrayBalloon(string text, BalloonIcon icon)
        {
            if (!Settings.Default.ShowTrayNotifications)
                return;
            App.NotifyIcon.ShowBalloonTip(Resources.AppName, text, icon);
        }

        public static void RefreshTrayIcon()
        {
            if (Settings.Default.AlwaysOn)
                App.NotifyIcon.ContextMenu = MenuHelper.NotifyMenu();
        }

        public static void CreateTrayIcon()
        {
            if (App.NotifyIcon != null)
                return;
            App.NotifyIcon = new TaskbarIcon {ToolTipText = Resources.AppName};
            var logo = new BitmapImage();
            logo.BeginInit();
            logo.UriSource =
                new Uri($"pack://application:,,,/SteamAccountSwitcher;component/steam.ico");
            logo.EndInit();
            App.NotifyIcon.IconSource = logo;
            App.NotifyIcon.Visibility = Settings.Default.AlwaysOn ? Visibility.Visible : Visibility.Hidden;
            App.NotifyIcon.TrayMouseDoubleClick += (sender, args) => SwitchWindowHelper.ShowSwitcherWindow();
            App.Accounts.CollectionChanged += (sender, args) => RefreshTrayIcon();

            RefreshTrayIcon();
        }
    }
}