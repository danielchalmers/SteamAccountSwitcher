#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Mutex _mutex = new Mutex(false,
            $"{SteamAccountSwitcher.Properties.Resources.AppName} SingleInstance");

        public static List<Account> Accounts;

        public App()
        {
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (Settings.Default.AlwaysOn)
                if (!e.Args.Contains("--restarting") && !_mutex.WaitOne(TimeSpan.Zero))
                {
                    Popup.Show("You can only run one instance at a time.");
                    Shutdown();
                }

            Init();
        }

        private void Init()
        {
            // Upgrade settings.
            SettingsHelper.UpgradeSettings();

            // Increment launch times.
            SettingsHelper.IncrementLaunches();

            // Add default shortcuts.
            if (ClickOnceHelper.IsFirstLaunch && Settings.Default.Accounts == string.Empty)
                Settings.Default.Accounts = AccountDataHelper.DefaultData();

            AccountDataHelper.ReloadData();

            // Resolve Steam path.
            SteamClient.ResolvePathShutdown();

            // Start update checker.
            UpdateCheckScheduler.Start();
            AutoSaveScheduler.Start();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            SettingsHelper.SaveSettings();
            ClickOnceHelper.RunOnStartup(Settings.Default.AlwaysOn);
        }

        private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var errorMessage = $"An unhandled exception occurred:\n\n{e.Exception.Message}";
            MessageBox.Show(errorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}