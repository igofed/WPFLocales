using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;

namespace SampleApplication.Library
{
    class Localization
    {
        public static readonly DependencyProperty DesignTimeLocaleProperty = DependencyProperty.RegisterAttached("DesignTimeLocale", typeof(string), typeof(Localization), new PropertyMetadata(null, OnDesignTimeLanguagePropertyChanged));
        public static void SetDesignTimeLocale(UIElement element, string value)
        {
            element.SetValue(DesignTimeLocaleProperty, value);
        }
        public static string GetDesignTimeLocale(UIElement element)
        {
            return (string)element.GetValue(DesignTimeLocaleProperty);
        }
        private static void OnDesignTimeLanguagePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (DesignTimeLanguages == null) DesignTimeLanguages = new Dictionary<DependencyObject, string>();

            var newLanguage = (string)dependencyPropertyChangedEventArgs.NewValue;
            DesignTimeLanguages[dependencyObject] = newLanguage;

            DesignTimeLanguageChanged(dependencyObject, newLanguage);
        }


        internal static event Action<DependencyObject, string> DesignTimeLanguageChanged = (d, l) => { };
        internal static Dictionary<DependencyObject, string> DesignTimeLanguages { get; set; }
    }

    [MarkupExtensionReturnType(typeof(string))]
    class LocalizableText : MarkupExtension
    {
        private readonly string _title = "";
        private DependencyObject _targetObject;
        private DependencyProperty _targetProperty;
        private DependencyObject _designTimeParent;

        public LocalizableText(string title)
        {
            _title = title;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var providerValuetarget = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));
            if (providerValuetarget != null)
            {
                _targetObject = (DependencyObject)providerValuetarget.TargetObject;
                _targetProperty = (DependencyProperty)providerValuetarget.TargetProperty;

                if (DesignerProperties.GetIsInDesignMode(_targetObject))
                {
                    if (_designTimeParent == null)
                    {
                        Localization.DesignTimeLanguageChanged += OnLocalizationDesignTimeLanguageChanged;

                        _designTimeParent = FindDesignTimeLanguage();
                    }

                    if (_designTimeParent != null)
                    {
                        var language = Localization.DesignTimeLanguages[_designTimeParent];
                        return language;
                    }

                    return _title;
                }
                else
                {
                    return _title;
                }
            }
            return _title;
        }

        private void OnLocalizationDesignTimeLanguageChanged(DependencyObject designTimeParent, string newLanguage)
        {
            if (_designTimeParent == null)
            {
                _designTimeParent = FindDesignTimeLanguage();
            }

            if (designTimeParent == _designTimeParent)
            {
                _targetObject.SetValue(_targetProperty, newLanguage);
            }
        }

        private DependencyObject FindDesignTimeLanguage()
        {
            var currentObject = _targetObject;
            do
            {
                if (Localization.DesignTimeLanguages.ContainsKey(currentObject))
                {
                    return currentObject;
                }
            } while ((currentObject = LogicalTreeHelper.GetParent(currentObject)) != null);

            return null;
        }
    }
}
