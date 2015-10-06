#region

using System.Runtime.InteropServices;

#endregion

namespace SteamAccountSwitcher
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Win32Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }
}