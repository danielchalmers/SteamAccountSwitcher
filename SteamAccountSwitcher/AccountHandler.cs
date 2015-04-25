#region

using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using SteamAccountSwitcher.Properties;

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
			_stackPanel = stackPanel;
			_accounts = Deserialize() ?? new List<Account>();
			Refresh();
		}

		public string Serialize()
		{
			return new Encryption().Encrypt(JsonConvert.SerializeObject(_accounts));
		}

		public List<Account> Deserialize()
		{
			return string.IsNullOrWhiteSpace(Settings.Default.Accounts)
				? new List<Account>()
				: JsonConvert.DeserializeObject<List<Account>>(new Encryption().Decrypt(Settings.Default.Accounts));
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
					Content =
						new TextBlock
						{
							Text = string.IsNullOrWhiteSpace(account.DisplayName) ? account.Username : account.DisplayName,
							TextWrapping = TextWrapping.Wrap
						},
					Height = 32,
					HorizontalContentAlignment = HorizontalAlignment.Left,
					Margin = new Thickness(0, 0, 0, 4),
					Padding = new Thickness(4, 0, 0, 0),
					ContextMenu = DefaultMenu()
				};
				btn.Click += Button_Click;
				btn.MouseEnter += delegate { SelectedIndex = _stackPanel.Children.IndexOf(btn); };

				_stackPanel.Children.Add(btn);
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var worker = new BackgroundWorker();
			worker.DoWork += worker_DoWork;
			worker.RunWorkerAsync();
		}

		private void worker_DoWork(object sender, DoWorkEventArgs e)
		{
			Steam.LogOut();
			Thread.Sleep(3000);
			Steam.LogIn(_accounts[SelectedIndex]);
		}

		private void OpenPropeties()
		{
			var dialog = new AccountProperties(_accounts[SelectedIndex]);
			dialog.ShowDialog();
			if (!string.IsNullOrWhiteSpace(dialog.NewAccount.Username))
				_accounts[SelectedIndex] = dialog.NewAccount;
			Refresh();
		}

		public void MoveUp(int i = -2)
		{
			var index = i == -2 ? SelectedIndex : i;
			if (index == 0) return;
			Swap(_accounts, index, index - 1);
			Refresh();
		}

		public void MoveDown(int i = -2)
		{
			var index = i == -2 ? SelectedIndex : i;
			if (_accounts.Count <= index + 1) return;
			Swap(_accounts, index, index + 1);
			Refresh();
		}

		public void Remove(int i = -2, bool msg = false)
		{
			var index = i == -2 ? SelectedIndex : i;
			if (index < 0 || index > _accounts.Count - 1) return;
			if (msg &&
			    Popup.Show($"Are you sure you want to remove this account?\r\n\r\n\"{_accounts[index].Username}\"",
				    MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes) == MessageBoxResult.No)
				return;
			_accounts.RemoveAt(index);
			Refresh();
		}

		private static void Swap<T>(IList<T> list, int indexA, int indexB)
		{
			var tmp = list[indexA];
			list[indexA] = list[indexB];
			list[indexB] = tmp;
		}

		private ContextMenu DefaultMenu()
		{
			var contextMenu = new ContextMenu();

			var item1 = new MenuItem {Header = "Properties..."};
			item1.Click += delegate { OpenPropeties(); };

			var item2 = new MenuItem {Header = "Move Up"};
			item2.Click += delegate { MoveUp(); };

			var item3 = new MenuItem {Header = "Move Down"};
			item3.Click += delegate { MoveDown(); };

			var item4 = new MenuItem {Header = "Remove..."};
			item4.Click += delegate { Remove(-2, true); };

			var item8 = new MenuItem {Header = "Exit"};
			item8.Click += delegate { Application.Current.Shutdown(); };

			contextMenu.Items.Add(item1);
			contextMenu.Items.Add(new Separator());
			contextMenu.Items.Add(item2);
			contextMenu.Items.Add(item3);
			contextMenu.Items.Add(new Separator());
			contextMenu.Items.Add(item4);
			contextMenu.Items.Add(new Separator());
			contextMenu.Items.Add(item8);

			return contextMenu;
		}
	}
}