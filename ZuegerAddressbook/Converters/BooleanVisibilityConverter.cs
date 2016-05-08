using System;
using System.Windows;
using System.Windows.Data;

namespace ZuegerAdressbook.Converters
{
    public class BooleanVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }

            var boolValue = (bool)value;
            if (boolValue)
            {
                return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var visibilityValue = (Visibility)value;
            if (visibilityValue == Visibility.Visible)
            {
                return true;
            }

            return false;
        }
    }
}
