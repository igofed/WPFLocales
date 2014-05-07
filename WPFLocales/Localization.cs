using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;
using WPFLocales.Utils;
using WPFLocales.Xml;

namespace WPFLocales
{
    public class Localization
    {
        #region DesignTime
        public static readonly DependencyProperty DesignTimeLocaleProperty = DependencyProperty.RegisterAttached("DesignTimeLocale", typeof(string), typeof(Localization), new PropertyMetadata(null, OnDesignTimeLocalePropertyChanged));
        public static void SetDesignTimeLocale(Control element, string value)
        {
            element.SetValue(DesignTimeLocaleProperty, value);
        }
        public static string GetDesignTimeLocale(Control element)
        {
            return (string)element.GetValue(DesignTimeLocaleProperty);
        }
        private static void OnDesignTimeLocalePropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            using (var writer = new StreamWriter("log.log", true))
            {
                writer.WriteLine("OnDesignTimeLocalePropertyChanged");
            }

            if (!DesignerProperties.GetIsInDesignMode(dependencyObject))
                return;

            if (DesignTimeLocales == null) DesignTimeLocales = new Dictionary<DependencyObject, string>();

            var newLanguage = (string)dependencyPropertyChangedEventArgs.NewValue;
            DesignTimeLocales[dependencyObject] = newLanguage;

            DesignTimeLocaleChanged(dependencyObject, newLanguage);

            var control = dependencyObject as Control;
            if (!control.IsLoaded)
            {
                control.Loaded += (sender, args) =>
                {
                    dependencyObject.UpdateBindingConverterParents();
                    dependencyObject.UpdateBindingTargets();
                };
            }
            else
            {
                dependencyObject.UpdateBindingTargets();
            }
        }

        public static readonly DependencyProperty DesignTimeLocalesPathProperty = DependencyProperty.RegisterAttached("DesignTimeLocalesPath", typeof(string), typeof(Localization), new PropertyMetadata(null, OnDesignTimeLocalesPathPropertyChanged));
        public static void SetDesignTimeLocalesPath(Control element, string value)
        {
            element.SetValue(DesignTimeLocalesPathProperty, value);
        }
        public static string GetDesignTimeLocalesPath(Control element)
        {
            return (string)element.GetValue(DesignTimeLocalesPathProperty);
        }
        private static void OnDesignTimeLocalesPathPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            using (var writer = new StreamWriter("log.log", true))
            {
                writer.WriteLine("OnDesignTimeLocalesPathPropertyChanged");
            }

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
                keyValue.Key.UpdateBindingTargets();
            }
        }

        internal static event Action<DependencyObject, string> DesignTimeLocaleChanged = (d, l) => { };
        internal static Dictionary<DependencyObject, string> DesignTimeLocales { get; set; }
        internal static Dictionary<DependencyObject, DependencyObject> DesignTimeLocaleParents { get; set; }
        internal static Dictionary<string, Dictionary<string, Dictionary<string, string>>> DesignTimeLocaleDictionary { get; set; }

        internal static string GetTextByKey(DependencyObject element, Enum key)
        {
            var text = "No locale available or design time locale didn't specified";

            if (element == null)
                return text;

            DependencyObject localeParent;
            if (!DesignTimeLocaleParents.TryGetValue(element, out localeParent))
            {
                localeParent = FindDesignTimeLocaleParent(element);
                if (localeParent != null)
                    DesignTimeLocaleParents[element] = localeParent;
            }

            if (localeParent == null)
                return text;

            var locale = DesignTimeLocales[localeParent];
            var groupKey = key.GetType().Name;
            var itemKey = key.ToString();

            if (DesignTimeLocaleDictionary.ContainsKey(locale))
            {
                var groups = DesignTimeLocaleDictionary[locale];
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

        private static DependencyObject FindDesignTimeLocaleParent(DependencyObject element)
        {
            var currentElement = element;
            do
            {
                if (DesignTimeLocales.ContainsKey(currentElement))
                {
                    return currentElement;
                }
            } while ((currentElement = LogicalTreeHelper.GetParent(currentElement)) != null);

            return null;
        }

        #endregion

        static Localization()
        {
            DesignTimeLocaleDictionary = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            DesignTimeLocales = new Dictionary<DependencyObject, string>();
            DesignTimeLocaleParents = new Dictionary<DependencyObject, DependencyObject>();
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

                Application.Current.UpdateBindingTargets();
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
