using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;

namespace WPFLocales.View
{
    [MarkupExtensionReturnType(typeof(string))]
    public class LocalizableText : MarkupExtension
    {
        private DependencyObject _targetObject;
        private DependencyProperty _targetProperty;
        private bool _isDesignMode;

        public Enum Key
        {
            get
            {
                return _key;
            }
            set
            {
                if (value != null)
                {
                    _key = value;
                    if (_targetObject != null && _targetProperty != null)
                    {
                        string text;
                        if (_isDesignMode)
                        {
                            text = Localization.GetTextByKey(_targetObject, _key);
                        }
                        else
                        {
                            text = Localization.GetTextByKey(_key);
                        }
                        _targetObject.SetValue(_targetProperty, text);
                    }
                }
            }
        }
        private Enum _key;

        public LocalizableText()
        {

        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var providerValuetarget = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            _targetObject = (DependencyObject)providerValuetarget.TargetObject;
            _targetProperty = (DependencyProperty)providerValuetarget.TargetProperty;

            _isDesignMode = DesignerProperties.GetIsInDesignMode(_targetObject);

            string text = "Key not set yet";
            if (_isDesignMode)
            {
                Localization.DesignTimeLocaleChanged += OnLocalizationDesignTimeLocaleChanged;

                if (_key != null)
                    text = Localization.GetTextByKey(_targetObject, _key);
            }
            else
            {
                Localization.LocaleChanged += OnLocalizationLocaleChanged;

                if (_key != null)
                    text = Localization.GetTextByKey(_key);
            }

            return text;
        }

        private void OnLocalizationDesignTimeLocaleChanged(DependencyObject designTimeParent, string newLanguage)
        {
            if (_key != null)
                _targetObject.SetValue(_targetProperty, Localization.GetTextByKey(_targetObject, _key));

        }

        private void OnLocalizationLocaleChanged()
        {
            if (_key != null)
                _targetObject.SetValue(_targetProperty, Localization.GetTextByKey(_key));
        }
    }
}