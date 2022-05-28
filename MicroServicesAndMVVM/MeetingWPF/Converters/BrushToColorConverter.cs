using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MeetingWPF.Converters
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
                return FromString(hexColor);
            }

            return null;
        }

        private Color FromSolidColorBrush(SolidColorBrush solidColorBrush)
        {
            return solidColorBrush.Color;
        }

        private Color FromString(string hexColor)
        {
            return ((SolidColorBrush)new BrushConverter().ConvertFromString(hexColor)).Color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
