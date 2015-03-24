using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SampleApplication.Localization;
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
                    return GetLocalizedString(LocalizationKeys.SampleApplication.StateErrorText);
                if (state == State.Ok)
                    return GetLocalizedString(LocalizationKeys.SampleApplication.StateOkText);
            }
            return "";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
