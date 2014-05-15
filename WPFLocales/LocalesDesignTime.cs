using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Serialization;
using WPFLocales.Utils;
using WPFLocales.Xml;

namespace WPFLocales
{
    public partial class Locales
    {
        public static readonly DependencyProperty DesignTimeLocaleProperty = DependencyProperty.RegisterAttached("DesignTimeLocale", typeof(string), typeof(Locales), new PropertyMetadata(null, OnDesignTimeLocalePropertyChanged));
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
            if (!DesignerProperties.GetIsInDesignMode(dependencyObject))
                return;

            var control = dependencyObject as Control;
            if (control == null)
                return;

            var newLanguage = (string)dependencyPropertyChangedEventArgs.NewValue;
            DesignTimeLocales[dependencyObject] = newLanguage;

            if (!control.IsLoaded)
            {
                control.Loaded += (sender, args) =>
                {
                    dependencyObject.UpdateBindingConverterParents();
                    dependencyObject.UpdateBindingTargets();

                    CurrentLocaleChanged();
                };
            }
            else
            {
                dependencyObject.UpdateBindingTargets();

                CurrentLocaleChanged();
            }
        }

        public static readonly DependencyProperty DesignTimeLocalesPathProperty = DependencyProperty.RegisterAttached("DesignTimeLocalesPath", typeof(string), typeof(Locales), new PropertyMetadata(null, OnDesignTimeLocalesPathPropertyChanged));
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

            AvailableLocales = new ReadOnlyCollection<string>(locales);

            foreach (var keyValue in DesignTimeLocales)
            {
                CurrentLocaleChanged();
                keyValue.Key.UpdateBindingTargets();
            }
        }

        private static readonly Dictionary<DependencyObject, string> DesignTimeLocales = new Dictionary<DependencyObject, string>();
        private static readonly Dictionary<DependencyObject, DependencyObject> DesignTimeLocaleParents = new Dictionary<DependencyObject, DependencyObject>();
        private static readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> DesignTimeLocaleDictionary = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

        internal static string GetTextByLocaleKey(DependencyObject element, Enum key)
        {
            var text = "No locale available or design time locale didn't specified";

            if (element == null)
                return "Please reload designer";

            DependencyObject localeParent;
            if (!DesignTimeLocaleParents.TryGetValue(element, out localeParent))
            {
                localeParent = FindDesignTimeLocaleParent(element);
                if (localeParent != null)
                    DesignTimeLocaleParents[element] = localeParent;
            }

            if (localeParent == null)
                return "Design time locale didn't specified";

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
            } while ((currentElement = VisualTreeHelper.GetParent(currentElement)) != null);

            return null;
        }
    }
}
