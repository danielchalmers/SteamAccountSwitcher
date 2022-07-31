using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace SteamAccountSwitcher
{
    [ValueConversion(typeof(object), typeof(Type))]
    public class DataTypeConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.GetType();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider) => this;
    }
}