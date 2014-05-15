using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
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
        public Enum FormatKey { get { return _formatKey; } set { _formatKey = value; if(ParentDependencyObject != null) ParentDependencyObject.UpdateBindingTargets(); } }
        private Enum _formatKey;

        /// <summary>
        /// Element which is parent of Binding which uses current converter
        /// </summary>
        internal DependencyObject ParentDependencyObject { get; set; }

        private readonly bool _isInDesignMode;

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
           
            var text = _isInDesignMode ? Locales.GetTextByLocaleKey(ParentDependencyObject, key) : Locales.GetTextByLocaleKey(key);

            if (withFormating && FormatKey != null)
            {
                var format = _isInDesignMode ? Locales.GetTextByLocaleKey(ParentDependencyObject, FormatKey) : Locales.GetTextByLocaleKey(FormatKey);
                text = string.Format(format, text);

                using (var writer = new StreamWriter("log.log", true))
                {
                    writer.Write("Parent: " + (ParentDependencyObject == null ? "NULL" : ParentDependencyObject.ToString()));
                    writer.Write("; Text: " + text);
                    writer.WriteLine();
                }
            }

            return text;
        }
    }
}
