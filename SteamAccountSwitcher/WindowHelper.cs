#region

using System;
using System.Runtime.InteropServices;

#endregion

namespace SteamAccountSwitcher
{
    internal class WindowHelper
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        public static Win32App GetForegroundApp() => new Win32App(GetForegroundWindow());
    }
}