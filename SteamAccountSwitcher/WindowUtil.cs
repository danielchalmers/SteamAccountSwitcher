using System.Windows;

namespace SteamAccountSwitcher
{
    public static class WindowUtil
    {
        public static void ShowDialogOrBringToFront(this Window window)
        {
            if (window.IsVisible)
            {
                window.Activate();
            }
            else
            {
                window.ShowDialog();
            }
        }
    }
}