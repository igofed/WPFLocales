using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;
using WPFLocales.Xml;

namespace WPFLocales
{
    public class Localization
    {
        #region DesignTime
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
            if (!DesignerProperties.GetIsInDesignMode(dependencyObject))
                return;

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
            if (!DesignerProperties.GetIsInDesignMode(dependencyObject))
                return;

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

            Locales = new ReadOnlyCollection<string>(locales);

            foreach (var keyValue in DesignTimeLocales)
            {
                DesignTimeLocaleChanged(keyValue.Key, keyValue.Value);
            }
        }

        internal static event Action<DependencyObject, string> DesignTimeLocaleChanged = (d, l) => { };
        internal static Dictionary<DependencyObject, string> DesignTimeLocales { get; set; }
        internal static Dictionary<string, Dictionary<string, Dictionary<string, string>>> DesignTimeLocaleDictionary { get; set; }
        #endregion

        static Localization()
        {
            DesignTimeLocaleDictionary = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            DesignTimeLocales = new Dictionary<DependencyObject, string>();
        }

        #region ProductionTime
        public static void Initialize(DirectoryInfo localesPath, string defaultLocale, string currentLocale = null)
        {
            if (!localesPath.Exists || !localesPath.GetFiles().Any(f => f.FullName.EndsWith(".locale")))
                throw new NotSupportedException("Directory should exists or contain locale files (.locale)");

            _allLocalesDictionary = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

            var locales = new List<string>();

            var files = localesPath.GetFiles().Where(f => f.FullName.EndsWith(".locale"));
            foreach (var file in files)
            {
                using (var stream = new FileStream(file.FullName, FileMode.Open))
                {
                    var serializer = new XmlSerializer(typeof(XmlLocale));
                    var locale = (XmlLocale)serializer.Deserialize(stream);

                    if (locales.Contains(locale.Key))
                        throw new NotSupportedException(string.Format("Locale {0} already registered", locale.Key));
                    locales.Add(locale.Key);

                    _allLocalesDictionary[locale.Key] = new Dictionary<string, Dictionary<string, string>>();
                    foreach (var group in locale.Groups)
                    {
                        _allLocalesDictionary[locale.Key][group.Key] = new Dictionary<string, string>();
                        foreach (var item in group.Items)
                        {
                            _allLocalesDictionary[locale.Key][group.Key][item.Key] = item.Value;
                        }
                    }
                }
            }

            Locales = new ReadOnlyCollection<string>(locales);

            if (!Locales.Contains(defaultLocale))
                throw new NotSupportedException("No default locale in registered locales");

            DefaultLocale = defaultLocale;
            CurrentLocale = defaultLocale;
        }

        private static Dictionary<string, Dictionary<string, Dictionary<string, string>>> _allLocalesDictionary;
        private static string _currentLocale;

        public static event Action LocaleChanged = () => { }; 
        public static ReadOnlyCollection<string> Locales { get; private set; }
        public static string DefaultLocale { get; private set; }
        public static string CurrentLocale
        {
            get { return _currentLocale; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new NullReferenceException("Locale can't be null or empty");
                
                if (!Locales.Contains(value))
                    throw new NotSupportedException("Locale not presented in registered");
                
                if (_currentLocale == value)
                    return;

                _currentLocale = value;

                CurrentLocaleDictionary = _allLocalesDictionary[value];

                LocaleChanged();
            }
        }
        public static Dictionary<string, Dictionary<string, string>> CurrentLocaleDictionary { get; private set; }

        public static string GetTextByKey(Enum key)
        {
            var groupName = key.GetType().Name;
            var itemName = key.ToString();

            if (!string.IsNullOrEmpty(CurrentLocale))
            {
                Dictionary<string, string> group;
                if (CurrentLocaleDictionary.TryGetValue(groupName, out group))
                {
                    string item;
                    if (CurrentLocaleDictionary[groupName].TryGetValue(itemName, out item))
                    {
                        return item;
                    }
                    else
                    {
                        throw new NotSupportedException("Can't find item in current locale");
                    }
                }
                else
                {
                    throw new NotSupportedException("Can't find item in current locale");
                }
            }
            else
            {
                throw new NotSupportedException("Current locale didn't selected");
            }
        }
        #endregion
    }
}
