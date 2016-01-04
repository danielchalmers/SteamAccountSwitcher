using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Hardcodet.Wpf.TaskbarNotification;
using SteamAccountSwitcher.Properties;

namespace SteamAccountSwitcher
{
    internal static class TrayIconHelper
    {
        public static readonly ObservableCollection<object> AccountMenuItems = new ObservableCollection<object>();

        public static void ShowRunningInTrayBalloon()
        {
            ShowTrayBalloon(
                $"{Resources.AppName} is running in system tray.\nDouble click icon to show window.",
                BalloonIcon.Info);
        }

        private static void ShowTrayBalloon(string text, BalloonIcon icon)
        {
            App.TrayIcon.ShowBalloonTip(Resources.AppName, text, icon);
        }

        public static void CreateTrayIcon()
        {
            if (App.TrayIcon != null)
                return;
            App.TrayIcon = (TaskbarIcon) Application.Current.FindResource("TrayIcon");
            RefreshTrayIconMenu();
            ShowRunningInTrayBalloon();
        }

        public static void RefreshTrayIconMenu()
        {
            AccountMenuItems.Clear();
            var items = ContextMenuItems();
            foreach (var item in items)
                AccountMenuItems.Add(item);
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
                    var index = i;
                    var item = new MenuItem
                    {
                        Header =
                            string.IsNullOrWhiteSpace(App.Accounts[index].DisplayName)
                                ? App.Accounts[index].Username
                                : App.Accounts[index].DisplayName
                    };

                    if (Settings.Default.ColorCodeAccountMenuItems)
                    {
                        item.Background = new SolidColorBrush(App.Accounts[index].Color);
                        item.Foreground = new SolidColorBrush(App.Accounts[index].TextColor);
                    }

                    item.Click += delegate { App.Accounts[index].SwitchTo(false); };
                    menuList.Add(item);
                }
                menuList.Add(new Separator());
            }

            menuList.Add(itemAddAccount);
            menuList.Add(itemManageAccounts);
            menuList.Add(new Separator());
            menuList.Add(itemStartSteam);
            menuList.Add(itemExitSteam);
            menuList.Add(new Separator());
            return menuList;
        }
    }
}