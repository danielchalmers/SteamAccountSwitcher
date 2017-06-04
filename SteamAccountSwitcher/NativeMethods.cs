using System;
using System.Runtime.InteropServices;

namespace SteamAccountSwitcher
{
    public static class NativeMethods
    {
        private const int HWND_BROADCAST = 0xffff;
        public static readonly int WM_ACTIVATE = RegisterWindowMessage($"WM_ACTIVATE|{AssemblyInfo.Guid}");

        public static void ShowExistingInstance()
        {
            PostMessage(
                (IntPtr)HWND_BROADCAST,
                WM_ACTIVATE,
                IntPtr.Zero,
                IntPtr.Zero);
        }

        [DllImport("user32")]
        private static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern int RegisterWindowMessage(string message);
    }
}