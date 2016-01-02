#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Deployment.Application;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
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
        public static ObservableCollection<Account> Accounts;
        public static SwitchWindow SwitchWindow;
        public static TaskbarIcon NotifyIcon;
        public static SaveTimer SaveTimer;
        public static bool SuccessfullyLoaded;
        public static bool IsShuttingDown;

        public App()
        {
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
            Settings.Default.PropertyChanged += Settings_OnPropertyChanged;
        }

        private void Settings_OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName == nameof(Settings.Default.AlwaysOn))
                TrayIconHelper.RefreshTrayIconVisibility();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (ApplicationDeployment.IsNetworkDeployed &&
                AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData != null &&
                AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData.Length > 0)
                Arguments =
                    AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData[0].Split(',').ToList();
            else
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