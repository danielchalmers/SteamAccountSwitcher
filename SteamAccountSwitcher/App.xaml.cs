#region

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
        public static Mutex AppMutex;
        public static List<string> Arguments;
        public static TaskScheduler UpdateScheduler;
        public static List<Account> Accounts;

        public App()
        {
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Arguments = e.Args.ToList();
            AppInitHelper.Initialize();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                base.OnExit(e);

                SettingsHelper.SaveSettings();
                ClickOnceHelper.RunOnStartup(Settings.Default.AlwaysOn);
            }
            catch
            {
                // ignored
            }
        }

        private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Popup.Show(
               $"An unhandled exception occurred:\r\n\r\n{(Settings.Default.ShowFullErrors ? e.Exception.ToString() : e.Exception.Message)}",
               MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }
    }
}