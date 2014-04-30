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
        private DependencyObject _designTimeParent;

        public LocalizableText(Enum key)
        {
            _key = key;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var providerValuetarget = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            _targetObject = (DependencyObject)providerValuetarget.TargetObject;
            _targetProperty = (DependencyProperty)providerValuetarget.TargetProperty;


            var text = "No locale available or design time locale didn't specified";
            if (DesignerProperties.GetIsInDesignMode(_targetObject))
            {
                Localization.DesignTimeLocaleChanged += OnLocalizationDesignTimeLocaleChanged;
                _designTimeParent = FindDesignTimeLocaleParent();

                if (_designTimeParent != null)
                {
                    var locale = Localization.DesignTimeLocales[_designTimeParent];

                    text = GetDesignTimeText(locale);
                }
            }
            else
            {
                Localization.LocaleChanged += OnLocalizationLocaleChanged;

                text = GetProductionTimeText();
            }

            return text;
        }

        private void OnLocalizationDesignTimeLocaleChanged(DependencyObject designTimeParent, string newLanguage)
        {
            if (_designTimeParent == null)
            {
                _designTimeParent = FindDesignTimeLocaleParent();
            }

            if (designTimeParent == _designTimeParent)
            {
                _targetObject.SetValue(_targetProperty, GetDesignTimeText(newLanguage));
            }
        }

        private DependencyObject FindDesignTimeLocaleParent()
        {
            var currentObject = _targetObject;
            do
            {
                if (Localization.DesignTimeLocales.ContainsKey(currentObject))
                {
                    return currentObject;
                }
            } while ((currentObject = LogicalTreeHelper.GetParent(currentObject)) != null);

            return null;
        }

        private string GetDesignTimeText(string locale)
        {
            var text = "No such locale available";

            var groupKey = _key.GetType().Name;
            var itemKey = _key.ToString();

            if (Localization.DesignTimeLocaleDictionary.ContainsKey(locale))
            {
                var groups = Localization.DesignTimeLocaleDictionary[locale];
                if (groups.ContainsKey(groupKey))
                {
                    var fields = groups[groupKey];
                    if (fields.ContainsKey(itemKey))
                    {
                        text = fields[itemKey];
                    }
                }
            }

            return text;
        }


        private void OnLocalizationLocaleChanged()
        {
            _targetObject.SetValue(_targetProperty, GetProductionTimeText());
        }

        private string GetProductionTimeText()
        {
            return Localization.GetTextByKey(_key);
        }
    }
}