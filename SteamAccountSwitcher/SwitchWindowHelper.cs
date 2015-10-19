namespace SteamAccountSwitcher
{
    internal class SwitchWindowHelper
    {
        public static void ShowSwitcherWindow()
        {
            if (App.SwitchWindow == null)
                App.SwitchWindow = new SwitchWindow();
            App.SwitchWindow.Show();
        }

        public static void HideSwitcherWindow()
        {
            App.SwitchWindow.HideWindow();
        }
    }
}