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
        public ObservableCollection<Control> AccountMenuItems { get; } = new();

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

            icon.RefreshAccountMenuItems();
        }

        private void Accounts_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshAccountMenuItems();
        }

        public void ShowRunningInTrayNotification()
        {
            ShowNotification("Running in the tray", "Click the icon to see your accounts");
        }

        private void RefreshAccountMenuItems()
        {
            IEnumerable<Control> GetAccountMenuItems()
            {
                if (Accounts == null || Accounts.Count <= 0)
                    yield break;

                foreach (var account in Accounts)
                {
                    var item = new MenuItem
                    {
                        Header = account.ToString(),
                        Tag = "account",
                    };

                    if (Settings.Default.ColorCodeAccountMenuItems)
                    {
                        item.Foreground = new SolidColorBrush(account.TextColor);
                        item.Background = new SolidColorBrush(account.Color);
                    }

                    item.Click += (sender, args) => account.SwitchTo();
                    yield return item;
                }

                yield return new Separator();
            }

            AccountMenuItems.Clear();

            foreach (var item in GetAccountMenuItems())
                AccountMenuItems.Add(item);
        }
    }
}