using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;

namespace WPFLocales.View
{
    [MarkupExtensionReturnType(typeof(string))]
    public class LocalizableText : MarkupExtension
    {
        private readonly Enum _key;
        private DependencyObject _targetObject;
        private DependencyProperty _targetProperty;

        public LocalizableText(Enum key)
        {
            _key = key;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var providerValuetarget = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            _targetObject = (DependencyObject)providerValuetarget.TargetObject;
            _targetProperty = (DependencyProperty)providerValuetarget.TargetProperty;

            string text;
            if (DesignerProperties.GetIsInDesignMode(_targetObject))
            {
                Localization.DesignTimeLocaleChanged += OnLocalizationDesignTimeLocaleChanged;
                
                text = Localization.GetTextByKey(_targetObject, _key);
            }
            else
            {
                Localization.LocaleChanged += OnLocalizationLocaleChanged;

                text = Localization.GetTextByKey(_key);
            }

            return text;
        }

        private void OnLocalizationDesignTimeLocaleChanged(DependencyObject designTimeParent, string newLanguage)
        {
            var text = Localization.GetTextByKey(_targetObject, _key);
            _targetObject.SetValue(_targetProperty, text);
        }

        private void OnLocalizationLocaleChanged()
        {
            _targetObject.SetValue(_targetProperty, Localization.GetTextByKey(_key));
        }
    }
}