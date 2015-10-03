using System.Windows;

namespace SteamAccountSwitcher
{
    public static class EnumUtilities
    {
        public static HorizontalAlignment ToHorizontalAlignment(this TextHorizontalAlignment textHorizontalAlignment)
        {
            switch (textHorizontalAlignment)
            {
                case TextHorizontalAlignment.Left:
                    return HorizontalAlignment.Left;
                default:
                case TextHorizontalAlignment.Center:
                    return HorizontalAlignment.Center;
                case TextHorizontalAlignment.Right:
                    return HorizontalAlignment.Right;
            }
        }
    }
}