using System;
using System.Globalization;
using System.Windows.Data;

namespace Meeting.Wpf.Converters
{
    public class StringToFirstLetterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string text))
                return "user";

            if (string.IsNullOrWhiteSpace(text))
                return "user";

            return char.ToUpper(text[0]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
