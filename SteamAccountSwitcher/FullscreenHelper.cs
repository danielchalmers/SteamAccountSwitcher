using System.Linq;
using System.Windows.Forms;

namespace SteamAccountSwitcher
{
    internal class FullscreenHelper
    {
        public static bool DoesMonitorHaveFullscreenApp(Screen screen)
        {
            return WindowHelper.GetForegroundApp()
                .IsFullScreen(screen);
        }

        public static bool DoesMonitorHaveFullscreenApp(int index)
        {
            return DoesMonitorHaveFullscreenApp(index != -1
                ? Screen.AllScreens[index]
                : Screen.PrimaryScreen);
        }

        public static bool DoesAnyMonitorHaveFullscreenApp()
        {
            return Screen.AllScreens.Any(DoesMonitorHaveFullscreenApp);
        }
    }
}