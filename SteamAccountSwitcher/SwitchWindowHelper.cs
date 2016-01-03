﻿namespace SteamAccountSwitcher
{
    internal static class SwitchWindowHelper
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
            App.SwitchWindow.Show();
            App.SwitchWindow.Activate();
            App.SwitchWindow.Focus();
        }
    }
}