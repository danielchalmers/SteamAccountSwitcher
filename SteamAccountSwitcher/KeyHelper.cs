using System.Windows.Input;

namespace SteamAccountSwitcher
{
    internal class KeyHelper
    {
        public static int KeyToInt(Key key)
        {
            var num = 0;
            switch (key)
            {
                case Key.D1:
                    num = 1;
                    break;
                case Key.D2:
                    num = 2;
                    break;
                case Key.D3:
                    num = 3;
                    break;
                case Key.D4:
                    num = 4;
                    break;
                case Key.D5:
                    num = 5;
                    break;
                case Key.D6:
                    num = 6;
                    break;
                case Key.D7:
                    num = 7;
                    break;
                case Key.D8:
                    num = 8;
                    break;
                case Key.D9:
                    num = 9;
                    break;
                case Key.D0:
                    num = 10;
                    break;
            }
            return num;
        }
    }
}