using System;
using System.Runtime.InteropServices;
using System.Windows;
using SteamAccountSwitcher.Properties;

namespace SteamAccountSwitcher
{
    public static class SingleInstanceHelper
    {
        private const int HWND_BROADCAST = 0xFFFF;

        private static readonly int WM_SHOW_APP = RegisterWindowMessage($"WM_SHOW_APP|{AssemblyInfo.Guid}");

        [DllImport("user32")]
        private static extern bool SendMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32")]
        private static extern int RegisterWindowMessage(string msg);

        public static void ShowExistingInstance()
        {
            SendMessage(
                (IntPtr) HWND_BROADCAST,
                WM_SHOW_APP,
                IntPtr.Zero,
                IntPtr.Zero);
        }

        public static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_SHOW_APP)
            {
                if (Settings.Default.AlwaysOn)
                {
                    if (App.SwitchWindow == null || App.SwitchWindow.Visibility != Visibility.Visible)
                        TrayIconHelper.ShowRunningInTrayBalloon();
                    else
                        SwitchWindowHelper.ActivateSwitchWindow();
                }
                else
                {
                    SwitchWindowHelper.ActivateSwitchWindow();
                }
            }

            return IntPtr.Zero;
        }
    }
}