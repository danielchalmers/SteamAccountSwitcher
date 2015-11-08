using System;
using System.Windows.Threading;

namespace SteamAccountSwitcher
{
    public class SaveTimer
    {
        private readonly DispatcherTimer _timer;

        public SaveTimer(TimeSpan waitTime)
        {
            _timer = new DispatcherTimer {Interval = waitTime};
            _timer.Tick += delegate { SettingsHelper.SaveSettings(); };
        }

        public void DelaySave()
        {
            _timer.Stop();
            _timer.Start();
        }
    }
}