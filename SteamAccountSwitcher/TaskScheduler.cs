#region

using System;
using System.Windows.Threading;

#endregion

namespace SteamAccountSwitcher
{
    public class TaskScheduler
    {
        private readonly DispatcherTimer _taskTimer;

        public TaskScheduler()
        {
            _taskTimer = new DispatcherTimer();
        }

        public void ScheduleTask(Action action, bool condition, TimeSpan interval)
        {
            _taskTimer.Tick += (sender, args) => { if (condition) action(); };
            _taskTimer.Interval = interval;
        }

        public void Start() => _taskTimer.Start();
        public void Stop() => _taskTimer.Stop();
    }
}