namespace SteamAccountSwitcher
{
    public static class SwitchWindowHelper
    {
        public static void ShowSwitcherWindow()
        {
            App.SwitchWindow.Show();
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