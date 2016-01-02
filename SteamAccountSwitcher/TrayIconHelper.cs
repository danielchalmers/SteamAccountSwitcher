using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
            App.NotifyIcon.ShowBalloonTip(Resources.AppName, text, icon);
        }

        public static void RefreshTrayIconMenu()
        {
            if (Settings.Default.AlwaysOn)
                App.NotifyIcon.ContextMenu = ContextMenu();
        }

        public static void RefreshTrayIconVisibility()
        {
            if (App.NotifyIcon == null)
                return;
            App.NotifyIcon.Visibility = Settings.Default.AlwaysOn ? Visibility.Visible : Visibility.Hidden;
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
            App.NotifyIcon.TrayMouseDoubleClick += (sender, args) => SwitchWindowHelper.ActivateSwitchWindow();
            App.Accounts.CollectionChanged += (sender, args) => RefreshTrayIconMenu();

            RefreshTrayIconMenu();
            RefreshTrayIconVisibility();
            ShowRunningInTrayBalloon();
        }

        private static IEnumerable<object> ContextMenuItems()
        {
            var menuList = new List<object>();

            var itemAddAccount = new MenuItem {Header = "Add Account..."};
            itemAddAccount.Click += delegate { AccountHelper.New(); };

            var itemManageAccounts = new MenuItem {Header = "Manage Accounts"};
            itemManageAccounts.Click += delegate { SwitchWindowHelper.ShowSwitcherWindow(); };

            var itemExitSteam = new MenuItem {Header = "Exit Steam"};
            itemExitSteam.Click += delegate { SteamClient.LogOutAuto(); };

            var itemStartSteam = new MenuItem {Header = "Open Steam"};
            itemStartSteam.Click += delegate { SteamClient.Launch(); };

            if (App.Accounts != null && App.Accounts.Count > 0)
            {
                for (var i = 0; i < App.Accounts.Count; i++)
                {
                    var item = new MenuItem
                    {
                        Header =
                            string.IsNullOrWhiteSpace(App.Accounts[i].DisplayName)
                                ? App.Accounts[i].Username
                                : App.Accounts[i].DisplayName
                    };
                    var i1 = i;

                    item.Background = new SolidColorBrush(App.Accounts[i1].Color);
                    item.Foreground = new SolidColorBrush(App.Accounts[i1].TextColor);

                    item.Click += delegate { App.Accounts[i1].SwitchTo(); };
                    menuList.Add(item);
                }
                menuList.Add(new Separator());
            }

            menuList.Add(itemAddAccount);
            menuList.Add(itemManageAccounts);
            menuList.Add(new Separator());
            menuList.Add(itemStartSteam);
            menuList.Add(itemExitSteam);
            return menuList;
        }

        private static ContextMenu ContextMenu()
        {
            var menu = new ContextMenu();
            var items = new List<object>();
            items.AddRange(ContextMenuItems());
            items.Add(new Separator());
            items.AddRange((object[]) App.SwitchWindow.FindResource("MainMenuItems"));
            menu.ItemsSource = items;
            return menu;
        }
    }
}