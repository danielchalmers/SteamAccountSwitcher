#region

using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

#endregion

namespace SteamAccountSwitcher
{
    /// <summary>
    ///     Interaction logic for AccountProperties.xaml
    /// </summary>
    public partial class AccountProperties : Window
    {
        public Account NewAccount;

        public AccountProperties(Account account = null)
        {
            InitializeComponent();
            Title = account == null ? "New Account" : "Edit Account";
            if (account == null)
                account = new Account();
            txtDisplayName.Text = account.DisplayName;
            txtUsername.Text = account.Username;
            txtPassword.Password = account.Password;
            txtColor.SelectedColor = account.Color.Color;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            NewAccount = new Account
            {
                DisplayName = txtDisplayName.Text,
                Username = txtUsername.Text,
                Password = string.IsNullOrWhiteSpace(txtPassword.Password) ? txtPasswordText.Text : txtPassword.Password,
                Color = new SolidColorBrush(txtColor.SelectedColor)
            };
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
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

        private void txtPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            if (chkShowPassword.IsChecked == true)
                txtPasswordText.Focus();
        }

        private void txtPasswordText_GotFocus(object sender, RoutedEventArgs e)
        {
            if (chkShowPassword.IsChecked == false)
                txtPassword.Focus();
        }
    }
}