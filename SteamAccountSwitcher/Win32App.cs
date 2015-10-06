#region

using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

#endregion

namespace SteamAccountSwitcher
{
    public class Win32App
    {
        public Win32App(IntPtr value)
        {
            hWnd = value;
        }

        private IntPtr hWnd { get; }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowRect(IntPtr hWnd, out Win32Rect rc);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetShellWindow();

        public string GetTitle()
        {
            const int nChars = 256;
            var builder = new StringBuilder(nChars);
            return GetWindowText(hWnd, builder, nChars) > 0 ? builder.ToString() : null;
        }

        public Win32Rect GetBounds()
        {
            Win32Rect appBounds;
            GetWindowRect(hWnd, out appBounds);
            return appBounds;
        }

        public Screen GetScreen()
        {
            var bounds = GetBounds();
            return Screen.AllScreens
                .FirstOrDefault(scr => scr.Bounds.Contains(bounds.Left, bounds.Top));
        }

        public bool IsFullScreen()
        {
            var fullscreen = false;
            foreach (var screen in Screen.AllScreens.Where(IsFullScreen))
                fullscreen = true;
            return fullscreen;
        }

        public bool IsFullScreen(Screen screen)
        {
            return screen != null && GetBounds().Equals(new Win32Rect
            {
                Left = screen.Bounds.Left,
                Top = screen.Bounds.Top,
                Right = screen.Bounds.Right,
                Bottom = screen.Bounds.Bottom
            }) && !(hWnd.Equals(GetDesktopWindow()) || hWnd.Equals(GetShellWindow()));
        }
    }
}