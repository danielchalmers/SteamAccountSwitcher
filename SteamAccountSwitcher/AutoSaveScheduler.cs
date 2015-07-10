#region

using System;
using System.Windows.Threading;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    internal class AutoSaveScheduler
    {
        private static readonly DispatcherTimer _schedulerTimer = new DispatcherTimer();

        static AutoSaveScheduler()
        {
            _schedulerTimer.Tick += (sender, args) =>
            {
                if (Settings.Default.AlwaysOn)
                    SettingsHelper.SaveSettings();
            };
            _schedulerTimer.Interval = new TimeSpan(0, 30, 0);
        }

        public static void Start()
        {
            _schedulerTimer.Start();
        }

        public static void Stop()
        {
            _schedulerTimer.Stop();
        }
    }
}