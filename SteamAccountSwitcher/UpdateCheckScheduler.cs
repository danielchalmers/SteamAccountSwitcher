#region

using System;
using System.Windows.Threading;
using SteamAccountSwitcher.Properties;

#endregion

namespace SteamAccountSwitcher
{
    internal class UpdateCheckScheduler
    {
        private static readonly DispatcherTimer _updateTimer = new DispatcherTimer();

        static UpdateCheckScheduler()
        {
            _updateTimer.Tick += (sender, args) =>
            {
                if (ClickOnceHelper.IsUpdateable && Settings.Default.CheckForUpdates)
                    ClickOnceHelper.CheckForUpdates(true);
            };
            _updateTimer.Interval = new TimeSpan(3, 0, 0);
        }

        public static void Start()
        {
            _updateTimer.Start();
        }

        public static void Stop()
        {
            _updateTimer.Stop();
        }
    }
}