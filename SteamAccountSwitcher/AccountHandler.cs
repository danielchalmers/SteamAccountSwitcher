#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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
		private readonly Action _closeWindow;
		private readonly StackPanel _stackPanel;
		private int SelectedIndex = -1;

		public AccountHandler(StackPanel stackPanel, Action closeWindow)
		{
			_stackPanel = stackPanel;
			_accounts = Deserialize() ?? new List<Account>();
			_closeWindow = closeWindow;
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
			foreach (var btn in _accounts.Select(account => new Button
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
			}))
			{
				btn.Click += Button_Click;

				_stackPanel.Children.Add(btn);
			}
		}

		private void SetFocus(object sender)
		{
			SelectedIndex = _stackPanel.Children.IndexOf((Button) sender);
		}

		private void SetFocus(object sender, RoutedEventArgs e)
		{
			SetFocus(((ContextMenu) sender).PlacementTarget);
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			SetFocus(sender);
			var worker = new BackgroundWorker();
			worker.DoWork += worker_DoWork;
			worker.RunWorkerCompleted += worker_Completed;
			worker.RunWorkerAsync();
			_closeWindow();
		}

		private void worker_Completed(object sender, RunWorkerCompletedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void worker_DoWork(object sender, DoWorkEventArgs e)
		{
			var processes = Process.GetProcessesByName("steam");
			if (processes.Length > 0)
			{
				SteamClient.LogOut();
				Thread.Sleep(3000);
			}
			SteamClient.LogIn(_accounts[SelectedIndex]);
		}

		private void OpenPropeties()
		{
			var dialog = new AccountProperties(_accounts[SelectedIndex]);
			dialog.ShowDialog();
			if (!string.IsNullOrWhiteSpace(dialog.NewAccount.Username))
				_accounts[SelectedIndex] = dialog.NewAccount;
			Refresh();
		}

		public void New()
		{
			var dialog = new AccountProperties();
			dialog.ShowDialog();
			if (!string.IsNullOrWhiteSpace(dialog.NewAccount.Username))
				Add(dialog.NewAccount);
			Refresh();
		}

		public void MoveUp(int i = -2)
		{
			var index = i == -2 ? SelectedIndex : i;
			if (index == 0) return;
			_accounts.Swap(index, index - 1);
			Refresh();
		}

		public void MoveDown(int i = -2)
		{
			var index = i == -2 ? SelectedIndex : i;
			if (_accounts.Count <= index + 1) return;
			_accounts.Swap(index, index + 1);
			Refresh();
		}

		public void Remove(int i = -2)
		{
			var index = i == -2 ? SelectedIndex : i;
			if (index < 0 || index > _accounts.Count - 1) return;
			if (Popup.Show($"Are you sure you want to remove this account?\r\n\r\n\"{_accounts[index].Username}\"",
				MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes) == MessageBoxResult.No)
				return;
			_accounts.RemoveAt(index);
			Refresh();
		}

		public void OpenOptions()
		{
			// Open options window.
			var dialog = new Options();
			dialog.ShowDialog();
			Refresh();
		}

		private ContextMenu DefaultMenu()
		{
			var contextMenu = new ContextMenu();
			contextMenu.Opened += SetFocus;

			var item1 = new MenuItem {Header = "Properties..."};
			item1.Click += delegate { OpenPropeties(); };

			var item2 = new MenuItem {Header = "Move Up"};
			item2.Click += delegate { MoveUp(); };

			var item3 = new MenuItem {Header = "Move Down"};
			item3.Click += delegate { MoveDown(); };

			var item4 = new MenuItem {Header = "Remove..."};
			item4.Click += delegate { Remove(); };

			var item6 = new MenuItem {Header = "Options..."};
			item6.Click += delegate { OpenOptions(); };

			var item7 = new MenuItem {Header = "Check for Updates"};
			item7.Click += delegate { ClickOnceHelper.CheckForUpdates(); };

			var item8 = new MenuItem {Header = "Exit"};
			item8.Click += delegate { Application.Current.Shutdown(); };

			contextMenu.Items.Add(item1);
			contextMenu.Items.Add(new Separator());
			contextMenu.Items.Add(item2);
			contextMenu.Items.Add(item3);
			contextMenu.Items.Add(new Separator());
			contextMenu.Items.Add(item4);
			contextMenu.Items.Add(new Separator());
			contextMenu.Items.Add(item6);
			contextMenu.Items.Add(new Separator());
			contextMenu.Items.Add(item7);
			contextMenu.Items.Add(new Separator());
			contextMenu.Items.Add(item8);

			return contextMenu;
		}
	}
}