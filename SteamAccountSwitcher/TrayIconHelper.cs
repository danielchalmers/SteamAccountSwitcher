using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Hardcodet.Wpf.TaskbarNotification;
using SteamAccountSwitcher.Properties;

namespace SteamAccountSwitcher
{
    public static class TrayIconHelper
    {
        public static readonly ObservableCollection<object> AccountMenuItems = new ObservableCollection<object>();

        public static void ShowRunningInTrayBalloon()
        {
            ShowTrayBalloon("Running in tray\n" +
                "Double click icon to open", BalloonIcon.Info);
        }

        private static void ShowTrayBalloon(string text, BalloonIcon icon)
        {
            App.TrayIcon?.ShowBalloonTip(AssemblyInfo.Title, text, icon);
        }

        public static void CreateTrayIcon()
        {
            if (App.TrayIcon == null)
            {
                App.TrayIcon = (TaskbarIcon)Application.Current.FindResource("TrayIcon");
            }
            RefreshTrayIconMenu();
        }

        public static void RefreshTrayIconMenu()
        {
            AccountMenuItems.Clear();
            var items = GetAccountMenuItems();
            foreach (var item in items)
            {
                AccountMenuItems.Add(item);
            }
        }

        private static IEnumerable<Control> GetAccountMenuItems()
        {
            if (App.Accounts == null || App.Accounts.Count <= 0)
            {
                yield break;
            }
            foreach (var account in App.Accounts)
            {
                var item = new MenuItem { Header = account.GetDisplayName() };
                if (Settings.Default.ColorCodeAccountMenuItems)
                {
                    item.Foreground = new SolidColorBrush(account.TextColor);
                    item.Background = new SolidColorBrush(account.Color);
                }
                item.Click += (sender, args) => account.SwitchTo(false);
                yield return item;
            }
            yield return new Separator();
        }
    }
}