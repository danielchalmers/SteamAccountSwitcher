namespace SteamAccountSwitcher
{
    public static class SwitchWindowHelper
    {
        public static void ShowSwitcherWindow()
        {
            App.SwitchWindow.Show();
            if (App.SwitchWindow.WindowState == System.Windows.WindowState.Minimized)
            {
                App.SwitchWindow.WindowState = System.Windows.WindowState.Normal;
            }
        }

        public static void HideSwitcherWindow()
        {
            App.SwitchWindow.HideWindow();
        }

        public static void ActivateSwitchWindow()
        {
            ShowSwitcherWindow();
            App.SwitchWindow.Activate();
            App.SwitchWindow.Focus();
        }
    }
}