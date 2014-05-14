using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace WPFLocales.View
{
    public abstract class LocalizableConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// Element which is parent of Binding which uses current converter
        /// </summary>
        internal DependencyObject ParentDependencyObject { get; set; }

        private readonly bool _isDesignMode;

        protected LocalizableConverter()
        {
            _isDesignMode = DesignerProperties.GetIsInDesignMode(new DependencyObject());
        }

        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);
        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var target = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            if (!(target.TargetObject is Binding))
                throw new NotSupportedException("This converter should be used only as MarkupExtension and should be not created as resource");

            return this;
        }

        /// <summary>
        /// Return localized string for current key
        /// </summary>
        /// <param name="key">Key of localization</param>
        /// <returns></returns>
        protected string GetLocalizedString(Enum key)
        {
            var text = _isDesignMode ? Localization.GetTextByKey(ParentDependencyObject, key) : Localization.GetTextByKey(key);

            return text;
        }
    }
}
