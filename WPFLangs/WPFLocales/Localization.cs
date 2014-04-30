using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using WPFLocales.Model;
using WPFLocales.Xml;

namespace WPFLocales
{
    public class Localization
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

            Locales = new ReadOnlyCollection<string>(locales);

            foreach (var keyValue in DesignTimeLocales)
            {
                DesignTimeLocaleChanged(keyValue.Key, keyValue.Value);
            }
        }

        internal static event Action<DependencyObject, string> DesignTimeLocaleChanged = (d, l) => { };
        internal static Dictionary<DependencyObject, string> DesignTimeLocales { get; set; }
        internal static Dictionary<string, Dictionary<string, Dictionary<string, string>>> DesignTimeLocaleDictionary { get; set; }


        static Localization()
        {
            DesignTimeLocaleDictionary = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            DesignTimeLocales = new Dictionary<DependencyObject, string>();
        }


        public static ReadOnlyCollection<string> Locales { get; private set; }
        public static string CurrentLocale { get; set; }
        public static Dictionary<string, Dictionary<string, string>> CurrentLocaleDictionary { get; set; }


        public static string GetTextByKey(Enum key)
        {
            var groupName = key.GetType().Name;
            var itemName = key.ToString();

            return null;
        }

        public static void RegisterLocaleClass(ILocale locale)
        {
            
        }

        public static void RegisterLocaleFile(FileInfo file)
        {
            
        }

        public static void RegisterLocalesDirectory(DirectoryInfo directory)
        {
            
        }
    }
}
