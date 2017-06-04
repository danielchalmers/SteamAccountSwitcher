#region

using System.Windows;

#endregion

namespace SteamAccountSwitcher
{
    public static class Popup
    {
        public static MessageBoxResult Show(string text, MessageBoxButton button = MessageBoxButton.OK,
            MessageBoxImage image = MessageBoxImage.Information, MessageBoxResult defaultButton = MessageBoxResult.OK)
        {
            try
            {
                return MessageBox.Show(text, AssemblyInfo.Title, button, image, defaultButton);
            }
            catch
            {
                return MessageBoxResult.Cancel;
            }
        }
    }
}