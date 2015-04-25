using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SteamAccountSwitcher
{
	/// <summary>
	/// Interaction logic for AccountProperties.xaml
	/// </summary>
	public partial class AccountProperties : Window
	{
		public Account NewAccount;

		public AccountProperties(Account account)
		{
			InitializeComponent();
			txtDisplayName.Text = account.DisplayName;
			txtUsername.Text = account.Username;
			txtPassword.Text = account.Password;
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			NewAccount = new Account
			{
				DisplayName = txtDisplayName.Text,
				Username = txtUsername.Text,
				Password = txtPassword.Text,
			};
		}

		private void btnOK_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}
