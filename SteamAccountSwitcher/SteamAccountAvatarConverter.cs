using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SteamAccountSwitcher
{
    [ValueConversion(typeof(object), typeof(ImageSource))]
    public class SteamAccountAvatarConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string id)
            {
                var steamDirectory = SteamClient.FindInstallDirectory();

                if (steamDirectory != null)
                {
                    var avatarPath = Path.Combine(steamDirectory, "config", "avatarcache", id + ".png");

                    if (File.Exists(avatarPath))
                        return new BitmapImage(new Uri(avatarPath));
                }
            }

            return new BitmapImage();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}