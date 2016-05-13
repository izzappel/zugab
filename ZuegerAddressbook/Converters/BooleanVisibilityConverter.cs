using System;
using System.Windows;
using System.Windows.Data;

namespace ZuegerAdressbook.Converters
{
    public class BooleanVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool invertValue = false;
            var invertValueString = parameter as string;
            if (invertValueString != null && invertValueString.Equals(bool.TrueString, StringComparison.InvariantCultureIgnoreCase))
            {
                invertValue = true;
            }

            if (value == null)
            {
                return DependencyProperty.UnsetValue;
            }

            var boolValue = (bool)value;
            if (invertValue)
            {
                boolValue = !boolValue;
            }

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
