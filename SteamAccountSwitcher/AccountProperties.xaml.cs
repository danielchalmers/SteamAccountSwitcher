#region

using System;
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
        private readonly Account _oldAccount;
        private Brush _color;
        public Account NewAccount;

        public AccountProperties()
        {
            InitializeComponent();
        }

        public AccountProperties(Account account)
        {
            InitializeComponent();
            _oldAccount = account;
            txtDisplayName.Text = account.DisplayName;
            txtUsername.Text = account.Username;
            txtPassword.Password = account.Password;
            cbColor.Text = account.ColorText;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            NewAccount = new Account
            {
                DisplayName = txtDisplayName.Text,
                Username = txtUsername.Text,
                Password = string.IsNullOrWhiteSpace(txtPassword.Password) ? txtPasswordText.Text : txtPassword.Password,
                Color = _color,
                ColorText = cbColor.Text
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

        private Brush ConvertColor(string colorhex)
        {
            return (Brush) new BrushConverter().ConvertFromString(colorhex);
        }

        private void cbColor_SelectionChanged(object sender, EventArgs e)
        {
            switch (cbColor.Text)
            {
                case "Blue":
                    _color = ConvertColor(Properties.Resources.ColorBlue);
                    break;
                case "Green":
                    _color = ConvertColor(Properties.Resources.ColorGreen);
                    break;
                case "Orange":
                    _color = ConvertColor(Properties.Resources.ColorOrange);
                    break;
                case "Yellow":
                    _color = ConvertColor(Properties.Resources.ColorYellow);
                    break;
                case "Pink":
                    _color = ConvertColor(Properties.Resources.ColorPink);
                    break;
                case "Custom...":
                    var dia = new HexColorChooser(_oldAccount?.Color);
                    dia.ShowDialog();
                    _color = dia.Color;
                    break;
                default:
                    _color = null;
                    break;
            }
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