using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Meeting.Wpf.Converters
{
    public class BrushToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush solidColorBrush)
            {
                return FromSolidColorBrush(solidColorBrush);
            }
            else if (value is string hexColor)
            {
                Color? color = FromString(hexColor);
                return color is null ? DependencyProperty.UnsetValue : color;
            }

            return DependencyProperty.UnsetValue;
        }

        private Color FromSolidColorBrush(SolidColorBrush solidColorBrush)
        {
            return solidColorBrush.Color;
        }

        private Color? FromString(string hexColor)
        {
            object? solidColorBrush = new BrushConverter().ConvertFromString(hexColor);

            if (solidColorBrush is not null)
                return ((SolidColorBrush)solidColorBrush).Color;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
