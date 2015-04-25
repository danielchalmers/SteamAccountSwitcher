#region

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

#endregion

namespace SteamAccountSwitcher
{
	internal class AccountHandler
	{
		private readonly List<Account> _accounts;
		private readonly StackPanel _stackPanel;
		private int SelectedIndex = -1;

		public AccountHandler(StackPanel stackPanel)
		{
			_accounts = new List<Account>();
			_stackPanel = stackPanel;
		}

		public void LogOut()
		{
			Scripts.Run(Scripts.Close, new Account());
		}

		public void LogIn(Account account)
		{
			Scripts.Run(Scripts.Open, account);
		}

		public void Add(Account account)
		{
			_accounts.Add(account);
			Refresh();
		}

		public void Refresh()
		{
			// Remove all buttons.
			_stackPanel.Children.Clear();

			// Add new buttons with saved shortcut data
			foreach (var account in _accounts)
			{
				var btn = new Button
				{
					Content = new TextBlock {Text = account.Username, TextWrapping = TextWrapping.Wrap},
					Height = 32,
					HorizontalContentAlignment = HorizontalAlignment.Left,
					Margin = new Thickness(0, 0, 0, 8)
				};
				btn.Click += Shortcut_Click;
				btn.MouseEnter += Shortcut_Focus;

				_stackPanel.Children.Add(btn);
			}
		}

		private void Shortcut_Click(object sender, RoutedEventArgs e)
		{
			LogIn(_accounts[SelectedIndex]);
		}

		private void Shortcut_Focus(object sender, RoutedEventArgs e)
		{
			var button = (Button) sender;
			var index = _stackPanel.Children.IndexOf(button);
			SelectedIndex = index;
		}
	}
}