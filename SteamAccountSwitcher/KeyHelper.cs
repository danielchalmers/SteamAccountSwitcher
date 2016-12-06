using System.Windows.Input;

namespace SteamAccountSwitcher
{
    public static class KeyHelper
    {
        public static int KeyToInt(Key key)
        {
            switch (key)
            {
                case Key.D0:
                    return 0;
                case Key.D1:
                    return 1;
                case Key.D2:
                    return 2;
                case Key.D3:
                    return 3;
                case Key.D4:
                    return 4;
                case Key.D5:
                    return 5;
                case Key.D6:
                    return 6;
                case Key.D7:
                    return 7;
                case Key.D8:
                    return 8;
                case Key.D9:
                    return 9;
            }
            return -1;
        }
    }
}