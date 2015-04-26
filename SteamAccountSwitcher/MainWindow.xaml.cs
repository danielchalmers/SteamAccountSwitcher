#region

using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
	/// <summary>
	///     Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly AccountHandler _accountHandler;

		public MainWindow()
		{
			InitializeComponent();
			// Upgrade settings from old version.
			if (Settings.Default.MustUpgrade)
			{
				Settings.Default.Upgrade();
				Settings.Default.MustUpgrade = false;
				Settings.Default.Save();
			}
			_accountHandler = new AccountHandler(stackPanel, Hide);

			if (stackPanel.Children.Count > 0)
				stackPanel.Children[0].Focus();

			// Restore window size.
			if (Settings.Default.Height <= MinHeight)
				Settings.Default.Height = 250;
			if (Settings.Default.Width <= MinWidth)
				Settings.Default.Width = 350;
			Height = Settings.Default.Height;
			Width = Settings.Default.Width;

			// Show "New Account" button if enabled in options.
			btnNewAccount.Visibility = Settings.Default.ShowNewAccountButton ? Visibility.Visible : Visibility.Collapsed;
			// Show "Add Account" button if enabled in options.
			btnAddAccount.Visibility = Settings.Default.ShowAddAccountButton ? Visibility.Visible : Visibility.Collapsed;

			// Add right click context menu.
			ContextMenu = DefaultMenu();
		}

		private ContextMenu DefaultMenu()
		{
			var menu = new ContextMenu();

			var itemAdd = new MenuItem {Header = "Add..."};
			itemAdd.Click += delegate { _accountHandler.New(); };
			var itemOptions = new MenuItem {Header = "Options..."};
			itemOptions.Click += delegate { _accountHandler.OpenOptions(); };
			var itemCheckUpdates = new MenuItem {Header = "Check for Updates"};
			itemCheckUpdates.Click += delegate { ClickOnceHelper.CheckForUpdates(); };
			var itemExit = new MenuItem {Header = "Exit"};
			itemExit.Click += delegate { Application.Current.Shutdown(); };

			menu.Items.Add(itemAdd);
			menu.Items.Add(new Separator());
			menu.Items.Add(itemOptions);
			menu.Items.Add(new Separator());
			menu.Items.Add(itemCheckUpdates);
			menu.Items.Add(new Separator());
			menu.Items.Add(itemExit);
			return menu;
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			SaveSettings();
		}

		public void SaveSettings()
		{
			// Save settings.
			Settings.Default.Height = Height;
			Settings.Default.Width = Width;
			Settings.Default.Accounts = _accountHandler.Serialize();
			Settings.Default.Save();
		}

		private void btnAddAccount_Click(object sender, RoutedEventArgs e)
		{
			_accountHandler.New();
		}

		private void btnNewAccount_Click(object sender, RoutedEventArgs e)
		{
			Process.Start("https://store.steampowered.com/join/");
		}
	}
}