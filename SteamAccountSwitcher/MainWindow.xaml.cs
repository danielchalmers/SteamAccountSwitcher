#region

using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
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
		}

		private void Window_Closing(object sender, CancelEventArgs e)
		{
			SaveSettings();
		}

		public void SaveSettings()
		{
			// Save settings.
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