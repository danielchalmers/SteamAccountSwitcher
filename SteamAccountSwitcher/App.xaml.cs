#region

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
        public static SwitchWindow SwitchWindow;
        public static TaskbarIcon TrayIcon;
        public static SaveTimer SaveTimer;

        public App()
        {
            Dispatcher.UnhandledException += OnDispatcherUnhandledException;
        }

        public static ObservableCollection<Account> Accounts { get; set; }
        private static bool SuccessfullyLoaded { get; set; }
        public static bool IsShuttingDown { get; set; }

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
                AppHelper.ShutdownApplication();
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
                AppHelper.SetRunOnStartup(Settings.Default.AlwaysOn);
            }
            catch
            {
                // ignored
            }
        }

        private static void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Popup.Show(
                SuccessfullyLoaded
                    ? $"An unhandled exception occurred:\n\n{e.Exception.Message}"
                    : $"A critical exception occurred:\n\n{e.Exception.Message}\n\nApplication will now exit.",
                MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
            if (!SuccessfullyLoaded)
                AppHelper.ShutdownApplication();
        }
    }
}