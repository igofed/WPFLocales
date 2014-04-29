using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Markup;
using System.Xml.Serialization;

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
            if (DesignTimeLocales == null) DesignTimeLocales = new Dictionary<DependencyObject, string>();

            var newLanguage = (string)dependencyPropertyChangedEventArgs.NewValue;
            DesignTimeLocales[dependencyObject] = newLanguage;

            DesignTimeLocaleChanged(dependencyObject, newLanguage);
        }

        public static readonly DependencyProperty DesignTimeLocalesPathProperty = DependencyProperty.RegisterAttached("DesignTimeLocalesPath", typeof(string), typeof(Localization), new PropertyMetadata(null, OnDesignTimeLocalesPathPropertyChanged));
        public static void SetDesignTimeLocalesPath(DependencyObject element, string value)
        {
            element.SetValue(DesignTimeLocalesPathProperty, value);
        }
        public static string GetDesignTimeLocalesPath(DependencyObject element)
        {
            return (string)element.GetValue(DesignTimeLocalesPathProperty);
        }
        private static void OnDesignTimeLocalesPathPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (dependencyPropertyChangedEventArgs.NewValue == null)
                return;


            var path = (string)dependencyPropertyChangedEventArgs.NewValue;

            var locales = new List<string>();

            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                using (var stream = new FileStream(file, FileMode.Open))
                {
                    var serializer = new XmlSerializer(typeof(XmlLocale));
                    var locale = (XmlLocale)serializer.Deserialize(stream);
                    locales.Add(locale.Key);

                    DesignTimeLocaleDictionary[locale.Key] = new Dictionary<string, Dictionary<string, string>>();
                    foreach (var group in locale.Groups)
                    {
                        DesignTimeLocaleDictionary[locale.Key][group.Key] = new Dictionary<string, string>();
                        foreach (var item in group.Items)
                        {
                            DesignTimeLocaleDictionary[locale.Key][group.Key][item.Key] = item.Value;
                        }
                    }
                }
            }

            using (var writer = new StreamWriter("file.log"))
            {
                writer.WriteLine(string.Join(";", locales));
            }

            Locales = new ReadOnlyCollection<string>(locales);
        }

        internal static event Action<DependencyObject, string> DesignTimeLocaleChanged = (d, l) => { };
        internal static Dictionary<DependencyObject, string> DesignTimeLocales { get; set; }
        internal static Dictionary<string, Dictionary<string, Dictionary<string, string>>> DesignTimeLocaleDictionary { get; set; }


        static Localization()
        {
            DesignTimeLocaleDictionary = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            DesignTimeLocales = new Dictionary<DependencyObject, string>();
        }

        public static ReadOnlyCollection<string> Locales { get; set; }
        public static string CurrentLocale { get; set; }
        public static Dictionary<string, Dictionary<string, string>> CurrentLocaleDictionary { get; set; }
    }

    [MarkupExtensionReturnType(typeof(string))]
    class LocalizableText : MarkupExtension
    {
        private readonly string _groupName;
        private readonly string _fieldName;
        private DependencyObject _targetObject;
        private DependencyProperty _targetProperty;
        private DependencyObject _designTimeParent;

        public LocalizableText(Enum title)
        {
            _groupName = title.GetType().Name;
            _fieldName = title.ToString();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var text = "No such locale available";

            var providerValuetarget = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget));

            _targetObject = (DependencyObject)providerValuetarget.TargetObject;
            _targetProperty = (DependencyProperty)providerValuetarget.TargetProperty;

            if (DesignerProperties.GetIsInDesignMode(_targetObject))
            {
                if (_designTimeParent == null)
                {
                    Localization.DesignTimeLocaleChanged += OnLocalizationDesignTimeLocaleChanged;

                    _designTimeParent = FindDesignTimeLocaleParent();
                }

                if (_designTimeParent != null)
                {
                    var language = Localization.DesignTimeLocales[_designTimeParent];

                    text = GetText(language);
                }

                return text;
            }
            else
            {
                return text;
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
                _targetObject.SetValue(_targetProperty, GetText(newLanguage));
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

        private string GetText(string language)
        {
            var text = "No such locale available";

            if (Localization.DesignTimeLocaleDictionary.ContainsKey(language))
            {
                var groups = Localization.DesignTimeLocaleDictionary[language];
                if (groups.ContainsKey(_groupName))
                {
                    var fields = groups[_groupName];
                    if (fields.ContainsKey(_fieldName))
                    {
                        text = fields[_fieldName];
                    }
                }
            }

            return text;
        }
    }
}
