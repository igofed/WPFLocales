using System;
using System.Globalization;
using WPFLocales.View;

namespace SampleApplication
{
    class StateToTextConverter : LocalizableConverter
    {
        public StateToTextConverter()
        {
            
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is State)
            {
                var state = (State)value;
                if (state == State.Error)
                    return GetLocalizedString(LocaleKeys.SampleApplication.StateErrorText);
                if(state == State.Ok)
                    return GetLocalizedString(LocaleKeys.SampleApplication.StateOkText);
            }
            return "";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
