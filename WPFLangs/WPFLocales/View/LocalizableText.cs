using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;

namespace WPFLocales.View
{
    [MarkupExtensionReturnType(typeof(string))]
    public class LocalizableText : MarkupExtension
    {
        private readonly string _groupKey;
        private readonly string _itemKey;
        private DependencyObject _targetObject;
        private DependencyProperty _targetProperty;
        private DependencyObject _designTimeParent;

        public LocalizableText(Enum key)
        {
            _groupKey = key.GetType().Name;
            _itemKey = key.ToString();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var providerValuetarget = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));

            _targetObject = (DependencyObject)providerValuetarget.TargetObject;
            _targetProperty = (DependencyProperty)providerValuetarget.TargetProperty;


            var text = "No such locale available";

            if (DesignerProperties.GetIsInDesignMode(_targetObject))
            {
                if (_designTimeParent == null)
                {
                    Localization.DesignTimeLocaleChanged += OnLocalizationDesignTimeLocaleChanged;

                    _designTimeParent = FindDesignTimeLocaleParent();
                }

                if (_designTimeParent != null)
                {
                    var locale = Localization.DesignTimeLocales[_designTimeParent];

                    text = GetDesignTimeText(locale);
                }

                return text;
            }
            else
            {
                return text;
            }
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

            if (Localization.DesignTimeLocaleDictionary.ContainsKey(locale))
            {
                var groups = Localization.DesignTimeLocaleDictionary[locale];
                if (groups.ContainsKey(_groupKey))
                {
                    var fields = groups[_groupKey];
                    if (fields.ContainsKey(_itemKey))
                    {
                        text = fields[_itemKey];
                    }
                }
            }

            return text;
        }

        private string GetProductionTimeText()
        {
            return "";
        }
    }
}