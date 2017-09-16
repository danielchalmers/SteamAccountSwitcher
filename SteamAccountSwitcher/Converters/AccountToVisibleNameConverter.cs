using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace SteamAccountSwitcher.Converters
{
    public class AccountToVisibleNameConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Account)value)?.GetDisplayName();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}