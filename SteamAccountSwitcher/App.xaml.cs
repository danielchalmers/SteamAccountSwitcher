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
        public static HelperWindow HelperWindow;
        public static MainWindow SwitchWindow;
        public static bool SuccessfullyLoaded;

        public App()
        {
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Arguments = e.Args.ToList();
            if (!AppInitHelper.Initialize())
            {
                ClickOnceHelper.ShutdownApplication();
                return;
            }
            MainWindow = SwitchWindow;
            SuccessfullyLoaded = true;
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
            var exception = (Settings.Default.ShowFullErrors
                ? e.Exception.ToString()
                : e.Exception.Message);
            Popup.Show(
                SuccessfullyLoaded
                    ? $"An unhandled exception occurred:\n\n{exception}"
                    : $"A critical exception occurred:\n\n{exception}\n\nApplication will now exit.",
                MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
            if (!SuccessfullyLoaded)
                ClickOnceHelper.ShutdownApplication();
        }
    }
}