using System;
using System.Threading;
using System.Windows.Threading;

namespace SteamAccountSwitcher
{
    public class SaveTimer
    {
        private readonly DispatcherTimer _timer;

        public SaveTimer(TimeSpan waitTime)
        {
            _timer = new DispatcherTimer { Interval = waitTime };
            _timer.Tick += Timer_OnTick;
        }

        private void Timer_OnTick(object sender, EventArgs eventArgs)
        {
            ThreadPool.QueueUserWorkItem(delegate { SettingsHelper.SaveSettings(); }, null);
        }

        public void DelaySave()
        {
            _timer.Stop();
            _timer.Start();
        }
    }
}