#region

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

#endregion

namespace SteamAccountSwitcher
{
    /// <summary>
    ///     Interaction logic for AccountProperties.xaml
    /// </summary>
    public partial class AccountProperties : Window
    {
        private Brush _color;
        public Account NewAccount;

        public AccountProperties()
        {
            InitializeComponent();
        }

        public AccountProperties(Account account)
        {
            InitializeComponent();
            txtDisplayName.Text = account.DisplayName;
            txtUsername.Text = account.Username;
            txtPassword.Password = account.Password;
            cbColor.Text = account.ColorText;
            _color = account.Color;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            NewAccount = new Account
            {
                DisplayName = txtDisplayName.Text,
                Username = txtUsername.Text,
                Password = string.IsNullOrWhiteSpace(txtPassword.Password) ? txtPasswordText.Text : txtPassword.Password,
                Color = GetSelectedColor(cbColor.Text),
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

        private Brush GetSelectedColor(string text)
        {
            switch (text)
            {
                case "Blue":
                    return ConvertColor(Properties.Resources.ColorBlue);
                case "Green":
                    return ConvertColor(Properties.Resources.ColorGreen);
                case "Orange":
                    return ConvertColor(Properties.Resources.ColorOrange);
                case "Yellow":
                    return ConvertColor(Properties.Resources.ColorYellow);
                case "Pink":
                    return ConvertColor(Properties.Resources.ColorPink);
                case "Custom...":
                    return _color;
                default:
                    return null;
            }
        }

        private void cbItem_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if ((((sender as ComboBoxItem).Content) as string) == "Custom...")
            {
                cbColor.Text = "Custom...";
                var dia = new HexColorChooser(_color);
                dia.ShowDialog();
                _color = dia.Color;
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