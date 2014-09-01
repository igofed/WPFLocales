using System;
using System.Globalization;
using System.Windows.Data;

namespace WPFLocales.View
{
    public class Formatter : LocalizableConverter, IMultiValueConverter
    {
        public Formatter(Enum formatKey)
        {
            FormatKey = formatKey;
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var format = GetLocalizedString(FormatKey, false);
            return string.Format(format, value);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Formatter doesn't support back convetation");
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var format = GetLocalizedString(FormatKey, false);
            return string.Format(format, values);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Formatter doesn't support back convetation");
        }
    }
}
