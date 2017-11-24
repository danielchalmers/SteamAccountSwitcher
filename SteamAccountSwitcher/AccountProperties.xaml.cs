using System.Windows;

namespace SteamAccountSwitcher
{
    /// <summary>
    ///     Interaction logic for AccountProperties.xaml
    /// </summary>
    public partial class AccountProperties : Window
    {
        public AccountProperties(Account account = null)
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;
            Title = $"{(account == null ? "Add" : "Edit")} Account";
            NewAccount = account == null ? new Account() : (Account)account.Clone();
            DataContext = NewAccount;
            txtPassword.Password = NewAccount.Password;
        }

        public Account NewAccount { get; }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (NewAccount.Password.Contains("\""))
            {
                Popup.Show("Your password cannot contain a double quote (\").", image: MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(NewAccount.Username))
            {
                Popup.Show("Please enter a username.", image: MessageBoxImage.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(NewAccount.Password))
            {
                Popup.Show("Please enter a password.", image: MessageBoxImage.Error);
                return;
            }
            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void chkShowPassword_Checked(object sender, RoutedEventArgs e)
        {
            txtPasswordText.Text = txtPassword.Password;
            txtPassword.Clear();
            txtPassword.Visibility = Visibility.Hidden;
            txtPasswordText.Visibility = Visibility.Visible;
            txtPasswordText.Focus();
        }

        private void chkShowPassword_Unchecked(object sender, RoutedEventArgs e)
        {
            txtPassword.Password = txtPasswordText.Text;
            txtPasswordText.Clear();
            txtPasswordText.Visibility = Visibility.Hidden;
            txtPassword.Visibility = Visibility.Visible;
            txtPassword.Focus();
        }

        private void txtPassword_OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            NewAccount.Password = txtPassword.Password;
        }

        private void txtPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            if (chkShowPassword.IsChecked == true)
            {
                txtPasswordText.Focus();
            }
        }

        private void txtPasswordText_GotFocus(object sender, RoutedEventArgs e)
        {
            if (chkShowPassword.IsChecked == false)
            {
                txtPassword.Focus();
            }
        }
    }
}