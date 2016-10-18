using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SteamAccountSwitcher
{
    public static class ConverterHelper
    {
        public static bool IsValueValid(IList<object> value, bool allowNull = false)
        {
            return value != null && value.All(x => x != DependencyProperty.UnsetValue && (allowNull || x != null));
        }

        public static bool IsValueValid(object value)
        {
            return value != null;
        }
    }
}