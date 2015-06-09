#region

using System;
using System.Collections.Generic;
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
        private Brush _color;
        private Account _oldAccount;

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

        private void cbColor_SelectionChanged(object sender, EventArgs e)
        {
            switch (cbColor.Text)
            {
                case "Blue":
                    _color = Brushes.LightBlue;
                    break;
                case "Green":
                    _color = Brushes.LightGreen;
                    break;
                case "Orange":
                    _color = Brushes.LightSalmon;
                    break;
                case "Pink":
                    _color = Brushes.LightPink;
                    break;
                case "Grey":
                    _color = Brushes.LightGray;
                    break;
                default:
                    var dia = new HexColorChooser(_oldAccount?.Color);
                    dia.ShowDialog();
                    _color = dia.Color;
                    break;
            }
        }
    }
}