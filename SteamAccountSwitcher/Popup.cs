using System.Windows;

namespace SteamAccountSwitcher
{
    public static class Popup
    {
        public static MessageBoxResult Show(
            string text,
            MessageBoxButton button = MessageBoxButton.OK,
            MessageBoxImage image = MessageBoxImage.Information,
            MessageBoxResult defaultButton = MessageBoxResult.OK)
        {
            return MessageBox.Show(
                text,
                AssemblyInfo.Title,
                button,
                image,
                defaultButton);
        }
    }
}