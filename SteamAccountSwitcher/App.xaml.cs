using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

        public static AccountCollection Accounts { get; set; }
        public static Mutex AppMutex { get; private set; }
        public static IReadOnlyList<string> Arguments { get; private set; }
        public static MyTaskbarIcon TrayIcon { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Arguments = e.Args.ToList();

            if (IsExistingInstanceRunning())
            {
                Shutdown(1);
                return;
            }

            SettingsHelper.UpgradeSettings();

            Accounts = SettingsHelper.DeserializeAccounts(Settings.Default.Accounts) ?? AccountCollection.Example;

            TrayIcon = (MyTaskbarIcon)FindResource("TrayIcon");

            if (Arguments.Contains("-systemstartup"))
            {
                if (!string.IsNullOrEmpty(Settings.Default.OnStartLoginName))
                {
                    Accounts.FirstOrDefault(x => x.Username == Settings.Default.OnStartLoginName)?.SwitchTo(onStart: true);
                }
            }
            else
            {
                TrayIcon.ShowRunningInTrayNotification();
            }

            Settings.Default.PropertyChanged += Settings_PropertyChanged;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            if (e.ApplicationExitCode == 0)
                SettingsHelper.SaveSettings();
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            TrayIcon.ShowNotification("An unhandled exception occurred", e.Exception.Message);
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Settings.Default.RunOnStartup))
            {
                SetRunOnStartup(Settings.Default.RunOnStartup);
            }
        }

        private bool IsExistingInstanceRunning()
        {
            AppMutex = new Mutex(true, AssemblyInfo.Guid, out var isNewInstance);

            return !isNewInstance;
        }

        private void SetRunOnStartup(bool runOnStartup)
        {
            var registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (runOnStartup)
            {
                registryKey?.SetValue(SteamAccountSwitcher.Properties.Resources.AppPathName, $"{ResourceAssembly.Location} -systemstartup");
            }
            else
            {
                registryKey?.DeleteValue(SteamAccountSwitcher.Properties.Resources.AppPathName, false);
            }
        }
    }
}