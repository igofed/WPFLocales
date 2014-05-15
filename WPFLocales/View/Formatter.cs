using System;
using System.Globalization;

namespace WPFLocales.View
{
    public class Formatter : LocalizableConverter
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
    }
}
