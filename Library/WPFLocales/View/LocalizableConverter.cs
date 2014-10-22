using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using WPFLocales.Utils;

namespace WPFLocales.View
{
    public abstract class LocalizableConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// FormatKey for converted value
        /// </summary>
        public Enum FormatKey
        {
            get { return _formatKey; }
            set { _formatKey = value; if(Parent != null) Parent.UpdateBindingTargets(); }
        }


        internal Control DesignLocaleParent { get; set; }
        internal DependencyObject Parent { get; set; }


        private readonly bool _isInDesignMode;
        private Enum _formatKey;


        protected LocalizableConverter()
        {
            _isInDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());
        }


        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);
        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }


        /// <summary>
        /// Return localized string for current key
        /// </summary>
        /// <param name="key">Key of localization</param>
        /// <param name="withFormating">Is use Format property to format output string. True by default</param>
        /// <returns></returns>
        protected string GetLocalizedString(Enum key, bool withFormating = true)
        {
            var text = _isInDesignMode ? Localization.GetTextByLocalizationKey(DesignLocaleParent, key) : Localization.GetTextByLocalizationKey(key);

            if (withFormating && FormatKey != null)
            {
                var format = _isInDesignMode ? Localization.GetTextByLocalizationKey(DesignLocaleParent, FormatKey) : Localization.GetTextByLocalizationKey(FormatKey);
                text = string.Format(format, text);
            }

            return text;
        }
    }
}
