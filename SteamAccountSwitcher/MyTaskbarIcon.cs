using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using H.NotifyIcon;
using SteamAccountSwitcher.Properties;

namespace SteamAccountSwitcher
{
    public class MyTaskbarIcon : TaskbarIcon
    {
        public readonly ObservableCollection<Control> AccountMenuItems = new();

        public ObservableCollection<Account> Accounts
        {
            get => (ObservableCollection<Account>)GetValue(AccountsProperty);
            set => SetValue(AccountsProperty, value);
        }

        public static readonly DependencyProperty AccountsProperty =
            DependencyProperty.Register(
                nameof(Accounts),
                typeof(ObservableCollection<Account>),
                typeof(MyTaskbarIcon),
                new PropertyMetadata(new ObservableCollection<Account>(), AccountsPropertyChanged));

        public MyTaskbarIcon()
        {
            ForceCreate();
        }

        public static void AccountsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var icon = (MyTaskbarIcon)d;
            var oldAccountsProperty = (ObservableCollection<Account>)e.OldValue;
            var newAccountsProperty = (ObservableCollection<Account>)e.NewValue;

            if (oldAccountsProperty != null)
                oldAccountsProperty.CollectionChanged -= icon.Accounts_CollectionChanged;

            if (newAccountsProperty != null)
                newAccountsProperty.CollectionChanged += icon.Accounts_CollectionChanged;
        }

        private void Accounts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            AccountMenuItems.Clear();

            foreach (var item in GetAccountMenuItems())
                AccountMenuItems.Add(item);
        }

        public void ShowRunningInTrayNotification()
        {
            ShowNotification("Running in tray", "Click the icon to see your accounts");
        }

        private static IEnumerable<Control> GetAccountMenuItems()
        {
            if (App.Accounts == null || App.Accounts.Count <= 0)
                yield break;

            foreach (var account in App.Accounts)
            {
                var item = new MenuItem
                {
                    Header = account.GetDisplayName(),
                    Tag = "account",
                };

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