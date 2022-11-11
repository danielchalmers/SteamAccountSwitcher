using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
using SteamAccountSwitcher.Properties;

namespace SteamAccountSwitcher
{
	/// <summary>
	///     Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		public App()
		{
			Dispatcher.UnhandledException += OnDispatcherUnhandledException;
		}

		public static Mutex AppMutex { get; private set; }
		public static MyTaskbarIcon TrayIcon { get; private set; }

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			AppMutex = new Mutex(true, "22E1FAEA-639E-400B-9DCB-F2D04EC126E1", out var isNewInstance);

			// Exit if another instance is already running.
			if (!isNewInstance)
			{
				Shutdown(1);
				return;
			}

			// Upgrade settings from an earlier version.
			if (Settings.Default.MustUpgrade)
			{
				Settings.Default.Upgrade();
				Settings.Default.MustUpgrade = false;
				Settings.Default.Save();
			}

			// Load tray icon.
			TrayIcon = (MyTaskbarIcon)FindResource("TrayIcon");
			TrayIcon.ShowRunningInTrayNotification();

			// Pre-render menu.
			TrayIcon.ContextMenu.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
			TrayIcon.ContextMenu.Arrange(new Rect(new Point(0, 0), TrayIcon.ContextMenu.DesiredSize));

			LoadAccounts();

			Settings.Default.PropertyChanged += Settings_PropertyChanged;
		}

		protected override void OnExit(ExitEventArgs e)
		{
			base.OnExit(e);

			AppMutex?.Dispose();
			TrayIcon?.Dispose();
		}

		private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			TrayIcon.ShowNotification("An unhandled error occurred", e.Exception.Message);
		}

		private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			switch (e.PropertyName)
			{
				case nameof(Settings.Default.RunOnStartup):
					SetRunOnStartup(Settings.Default.RunOnStartup);
					break;

				case nameof(Settings.Default.SteamInstallDirectory):
				case nameof(Settings.Default.ShowAvatars):
					LoadAccounts();
					break;
			}
		}

		private void LoadAccounts()
		{
			var steamDirectory = SteamClient.FindInstallDirectory();

			if (steamDirectory != null)
				SteamClient.Accounts.SetDirectory(steamDirectory);
		}

		private void SetRunOnStartup(bool runOnStartup)
		{
			using var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

			if (runOnStartup)
			{
				key?.SetValue(SteamAccountSwitcher.Properties.Resources.AppPathName, ResourceAssembly.Location);
			}
			else
			{
				key?.DeleteValue(SteamAccountSwitcher.Properties.Resources.AppPathName, false);
			}
		}
	}
}