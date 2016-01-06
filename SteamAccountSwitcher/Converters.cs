using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using SteamAccountSwitcher.Properties;

namespace SteamAccountSwitcher
{
    public class AccountToVisibleNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!ConverterHelper.IsValueValid(value))
                return DependencyProperty.UnsetValue;
            var val = value as Account;
            if (val == null)
                return DependencyProperty.UnsetValue;
            return val.GetDisplayName();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ColorToSolidBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!ConverterHelper.IsValueValid(value))
                return DependencyProperty.UnsetValue;
            return new SolidColorBrush((Color) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MultiBooleanToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!ConverterHelper.IsValueValid(value))
                return DependencyProperty.UnsetValue;
            return Settings.Default.AccountHelperButtons
                ? value.Any(val => (bool) val) ? Visibility.Visible : Visibility.Collapsed
                : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class NumberToIncrementedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!ConverterHelper.IsValueValid(value))
                return DependencyProperty.UnsetValue;
            return (int) value + 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IsExpandedToMoreOptionsHeaderConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!ConverterHelper.IsValueValid(value))
                return DependencyProperty.UnsetValue;
            return (bool) value ? "Less Options" : "More Options";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}