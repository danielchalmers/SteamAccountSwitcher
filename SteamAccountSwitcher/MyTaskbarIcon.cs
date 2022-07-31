using H.NotifyIcon;

namespace SteamAccountSwitcher
{
    public class MyTaskbarIcon : TaskbarIcon
    {
        public MyTaskbarIcon()
        {
            ForceCreate();
        }

        public void ShowRunningInTrayNotification()
        {
            ShowNotification("Running in the tray", "Click the icon to see your accounts");
        }
    }
}