using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Xml;
using SteamAccountSwitcher.Properties;

namespace SteamAccountSwitcher;

[ValueConversion(typeof(string), typeof(Image))]
public class SteamIdToAvatarConverter : MarkupExtension, IValueConverter
{
	public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
	{
		try
		{
			if (Settings.Default.ShowAvatars && value is string id)
			{
				var profileDocument = new XmlDocument();

				profileDocument.Load($"https://steamcommunity.com/profiles/{id}?xml=1");

				var avatarIconNode = profileDocument.DocumentElement.SelectSingleNode("/profile/avatarIcon");

				return new Image { Source = new BitmapImage(new Uri(avatarIconNode.InnerText)) };
			}
		}
		catch
		{
			// Couldn't load the image.
		}

		return null;
	}

	public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}

	public override object ProvideValue(IServiceProvider serviceProvider) => this;
}