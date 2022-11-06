using System;
using System.Globalization;
using System.IO;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using SteamAccountSwitcher.Properties;

namespace SteamAccountSwitcher;

[ValueConversion(typeof(object), typeof(object))]
public class SteamIdToAvatarConverter : MarkupExtension, IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		if (Settings.Default.ShowAvatars && value is string id)
		{
			var steamDirectory = SteamClient.FindInstallDirectory();

			if (steamDirectory != null)
			{
				var avatarPath = Path.Combine(steamDirectory, "config", "avatarcache", id + ".png");

				if (File.Exists(avatarPath))
					return new Image { Source = new BitmapImage(new Uri(avatarPath)) };
			}
		}

		return null;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public override object ProvideValue(IServiceProvider serviceProvider) => this;
}