#region

using System.ComponentModel;
using System.Windows;

#endregion

namespace SteamAccountSwitcher
{
	/// <summary>
	///     Interaction logic for AccountProperties.xaml
	/// </summary>
	public partial class AccountProperties : Window
	{
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
			txtPassword.Text = account.Password;
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			NewAccount = new Account
			{
				DisplayName = txtDisplayName.Text,
				Username = txtUsername.Text,
				Password = txtPassword.Text
			};
		}

		private void btnOK_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
		}
	}
}