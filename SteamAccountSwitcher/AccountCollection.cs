using System.Collections.ObjectModel;

namespace SteamAccountSwitcher
{
    public class AccountCollection : ObservableCollection<Account>
    {
        public void New()
        {
            var dialog = new AccountProperties();

            dialog.ShowDialog();

            if (dialog.DialogResult != true || dialog.NewAccount == null)
                return;

            if (string.IsNullOrWhiteSpace(dialog.NewAccount.Username) &&
                string.IsNullOrWhiteSpace(dialog.NewAccount.Password) &&
                string.IsNullOrWhiteSpace(dialog.NewAccount.DisplayName))
            {
                return;
            }

            Add(dialog.NewAccount);
        }

        public void Edit(Account account)
        {
            var dialog = new AccountProperties(account);
            dialog.ShowDialog();
            if (dialog.DialogResult != true)
                return;

            var newAccount = dialog.NewAccount;
            if (newAccount == null)
                return;

            var accountIndex = IndexOf(account);
            if (accountIndex == -1)
                return;

            this[accountIndex] = newAccount;
        }

        public static AccountCollection Example
        {
            get
            {
                var accounts = new AccountCollection
                {
                    Account.ExampleAccount
                };
                return accounts;
            }
        }
    }
}