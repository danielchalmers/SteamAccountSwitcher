using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SteamAccountSwitcher.Properties;

namespace SteamAccountSwitcher
{
    public class SwitchWindowViewModel : ViewModelBase
    {
        private bool _isAccountContextMenuOpen;

        public SwitchWindowViewModel()
        {
            Focus = new RelayCommand<Account>(FocusExecute);
            Account = new RelayCommand(AccountExecute);
            OpenAccountContextMenu = new RelayCommand(OpenAccountContextMenuExecute);
            Edit = new RelayCommand(EditExecute);
            MoveUp = new RelayCommand(MoveUpExecute);
            MoveDown = new RelayCommand(MoveDownExecute);
            Remove = new RelayCommand(RemoveExecute);
            Sort = new RelayCommand<string>(SortExecute);

            Options = new RelayCommand(OptionsExecute);
            AddAccount = new RelayCommand(AddAccountExecute);

            PreviewKeyUp = new RelayCommand<KeyEventArgs>(PreviewKeyUpExecute);

            AccountContextMenu = (ContextMenu) Application.Current.FindResource("AccountContextMenu");
            if (AccountContextMenu != null) AccountContextMenu.DataContext = this;
        }

        public Account SelectedAccount { get; set; }
        public ContextMenu AccountContextMenu { get; }

        public ICommand Focus { get; set; }
        public ICommand Account { get; set; }
        public ICommand OpenAccountContextMenu { get; set; }
        public ICommand Edit { get; set; }
        public ICommand MoveUp { get; set; }
        public ICommand MoveDown { get; set; }
        public ICommand Remove { get; set; }
        public ICommand Sort { get; set; }

        public ICommand Options { get; set; }
        public ICommand AddAccount { get; set; }

        public ICommand PreviewKeyUp { get; set; }

        public bool IsAccountContextMenuOpen
        {
            get { return _isAccountContextMenuOpen; }
            set
            {
                _isAccountContextMenuOpen = value;
                RaisePropertyChanged(nameof(IsAccountContextMenuOpen));
            }
        }

        private void PreviewKeyUpExecute(KeyEventArgs e)
        {
            if (!Settings.Default.NumberHotkeys)
                return;

            var num = KeyHelper.KeyToInt(e.Key);
            if (num > 0 && App.Accounts.Count >= num)
                App.Accounts[num - 1].SwitchTo();
        }

        private void OptionsExecute()
        {
            SettingsHelper.OpenOptions();
        }

        private void AddAccountExecute()
        {
            AccountHelper.New();
        }

        private void FocusExecute(Account account)
        {
            SelectedAccount = account;
        }

        private void AccountExecute()
        {
            SelectedAccount?.SwitchTo();
        }

        private void OpenAccountContextMenuExecute()
        {
            IsAccountContextMenuOpen = true;
        }

        private void EditExecute()
        {
            SelectedAccount?.Edit();
        }

        private void MoveUpExecute()
        {
            SelectedAccount?.MoveUp();
        }

        private void MoveDownExecute()
        {
            SelectedAccount?.MoveDown();
        }

        private void RemoveExecute()
        {
            SelectedAccount?.Remove(true);
        }

        private void SortExecute(string mode)
        {
            switch (mode)
            {
                case "AliasAscending":
                    App.Accounts.OrderBy(x => x.DisplayName).Reload();
                    break;
                case "UsernameAscending":
                    App.Accounts.OrderBy(x => x.Username).Reload();
                    break;
                case "AddDateAscending":
                    App.Accounts.OrderBy(x => x.AddDate).Reload();
                    break;
                case "LastModifiedAscending":
                    App.Accounts.OrderBy(x => x.LastModifiedDate).Reload();
                    break;
                case "AliasDescending":
                    App.Accounts.OrderByDescending(x => x.DisplayName).Reload();
                    break;
                case "UsernameDescending":
                    App.Accounts.OrderByDescending(x => x.Username).Reload();
                    break;
                case "AddDateDescending":
                    App.Accounts.OrderByDescending(x => x.AddDate).Reload();
                    break;
                case "LastModifiedDescending":
                    App.Accounts.OrderByDescending(x => x.LastModifiedDate).Reload();
                    break;
            }
        }
    }
}