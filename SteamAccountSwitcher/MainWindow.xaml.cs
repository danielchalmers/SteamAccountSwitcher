using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SteamAccountSwitcher.Properties;

namespace SteamAccountSwitcher
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private AccountHandler _accountHandler;
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
			_accountHandler = new AccountHandler(stackPanel);
		}

		private void btnAdd_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new AccountProperties(new Account());
			dialog.ShowDialog();
			_accountHandler.Add(dialog.NewAccount);
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
	}
}
