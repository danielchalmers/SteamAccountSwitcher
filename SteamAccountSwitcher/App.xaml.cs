using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Deployment.Application;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using SteamAccountSwitcher.Properties;

namespace SteamAccountSwitcher
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Mutex AppMutex;
        public static List<string> Arguments;
        public static SwitchWindow SwitchWindow;
        public static TaskbarIcon TrayIcon;

        public App()
        {
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        public static ObservableCollection<Account> Accounts { get; set; }
        public static bool IsShuttingDown { get; set; }
        public static bool HasInitialized { get; private set; } = false;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (ApplicationDeployment.IsNetworkDeployed &&
                AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData != null &&
                AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData.Length > 0)
            {
                Arguments =
                    AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData[0].Split(',').ToList();
            }
            else
            {
                Arguments = e.Args.ToList();
            }

            if (!AppInitHelper.Initialize())
            {
                AppHelper.Shutdown();
                return;
            }
            MainWindow = SwitchWindow;

            Settings.Default.PropertyChanged += Settings_PropertyChanged;

            HasInitialized = true;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            SettingsHelper.SaveSettings();
        }

        private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Popup.Show(
                "An unhandled exception occurred:\n\n" +
                e.Exception,
                MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Settings.Default.RunOnStartup))
            {
                AppHelper.SetRunOnStartup(Settings.Default.RunOnStartup);
            }
        }

        public static void ShowMainWindow()
        {
            SwitchWindow.Show();
            if (SwitchWindow.WindowState == WindowState.Minimized)
            {
                SwitchWindow.WindowState = WindowState.Normal;
            }
        }

        public static void HideMainWindow()
        {
            SwitchWindow.Hide();
        }

        public static void ShowAndActivateMainWindow()
        {
            ShowMainWindow();
            SwitchWindow.Activate();
            SwitchWindow.Focus();
        }
    }
}