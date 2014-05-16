using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml.Serialization;
using WPFLocales.Utils;
using WPFLocales.View;
using WPFLocales.Xml;

namespace WPFLocales
{
    public partial class Locales
    {
        /// <summary>
        /// Attached property for design time locale for current control, locale should be in list of locales availbale by DesignTimeLocalesPath path
        /// </summary>
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

            var parentControl = dependencyObject as Control;
            if (parentControl == null)
                return;

            var newLocale = (string)dependencyPropertyChangedEventArgs.NewValue;
            //need to store locale for each parent's control
            DesignTimeLocales[parentControl] = newLocale;

            //on loading process we have no access to full XAML tree, so need to wait for Loaded, when tree builded
            if (!parentControl.IsLoaded)
            {
                parentControl.Loaded += OnControlLoaded; 
            }
            else
            {
                //if current control is loaded, then binding's converters's parents already setted
                //and we need only trigger bindings to update their targets
                dependencyObject.UpdateBindingTargets();
                //udpate all localized texts
                UpdateLocalizedTextsForControl(parentControl);
            }
        }
        private static void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            var control = sender as Control;
            if (control == null)
                return;

            //unsubsribe from event
            control.Loaded -= OnControlLoaded;
            //update LocalizableConverter parents for design time work
            control.UpdateBindingConverterParents();
            //update binding's targets for requesting new values
            control.UpdateBindingTargets();
            //udpate all localized texts
            UpdateLocalizedTextsForControl(control);
        }

        /// <summary>
        /// Attached property for design time locale's path for current control. .locale files should be accessible by this path
        /// </summary>
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

            //reading all locales from XMLs
            var files = Directory.GetFiles(path).Where(f => f.EndsWith(".locale")); ;
            foreach (var file in files)
            {
                using (var stream = new FileStream(file, FileMode.Open))
                {
                    var serializer = new XmlSerializer(typeof(XmlLocale));
                    var locale = (XmlLocale)serializer.Deserialize(stream);
                    locales.Add(locale.Key);

                    //filling locales fictionaries
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

            //locales changed - need to update all localized 
            foreach (var keyValue in DesignTimeLocales)
            {
                keyValue.Key.UpdateBindingTargets();

                UpdateLocalizedTextsForControl(keyValue.Key);
            }
        }

        private static readonly Dictionary<Control, string> DesignTimeLocales = new Dictionary<Control, string>();
        private static readonly Dictionary<DependencyObject, Control> DesignTimeLocaleParents = new Dictionary<DependencyObject, Control>();
        private static readonly Dictionary<string, Dictionary<string, Dictionary<string, string>>> DesignTimeLocaleDictionary = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
        private static readonly Dictionary<Control, List<LocalizableText>> ControlsTexts = new Dictionary<Control, List<LocalizableText>>();
        private static readonly Dictionary<DependencyObject, LocalizableText> WaitingForRegister = new Dictionary<DependencyObject, LocalizableText>();
        
        //returns localized text for element and localization key
        internal static string GetTextByLocaleKey(DependencyObject element, Enum localicationKey)
        {
            //can occur for Converter which FormatKey was changed
            //just need reload designer for settings them back
            if (element == null)
                return "Please reload designer";

            //looking for elemtn's parent with setted locale
            Control localeParent;
            if (!DesignTimeLocaleParents.TryGetValue(element, out localeParent))
            {
                localeParent = FindDesignTimeLocaleParent(element);
                if (localeParent != null)
                    DesignTimeLocaleParents[element] = localeParent;
            }

            //no such parent
            if (localeParent == null)
                return "Design time locale didn't specified";

            var locale = DesignTimeLocales[localeParent];
            var groupKey = localicationKey.GetType().Name;
            var itemKey = localicationKey.ToString();

            //looking for item in dictionary
            var item = "No locale available or design time locale didn't specified";
            if (DesignTimeLocaleDictionary.ContainsKey(locale))
            {
                var groups = DesignTimeLocaleDictionary[locale];
                if (groups.ContainsKey(groupKey))
                {
                    var fields = groups[groupKey];
                    if (fields.ContainsKey(itemKey))
                    {
                        item = fields[itemKey];
                    }
                }
            }
            return item;
        }

        //registers LocalizableText for future or adds it to Waiting collection
        //waiting collection occurs when parent is not know at time of calling this
        internal static void RegisterLocalizableTextForElement(LocalizableText localizableText, DependencyObject element)
        {
            var parent = FindDesignTimeLocaleParent(element);
            if (parent != null)
            {
                if (!ControlsTexts.ContainsKey(parent))
                    ControlsTexts[parent] = new List<LocalizableText>();
                ControlsTexts[parent].Add(localizableText);
            }
            else
            {
                WaitingForRegister.Add(element, localizableText);
            }
        }

        //registers all waiting controls 
        private static void RegisterWaitingForRegister()
        {
            if (!WaitingForRegister.Any())
                return;

            var waiting = WaitingForRegister.ToList();
            foreach (var keyValuePair in waiting)
            {
                var parent = FindDesignTimeLocaleParent(keyValuePair.Key);
                if (parent == null) continue;

                if (!ControlsTexts.ContainsKey(parent))
                    ControlsTexts[parent] = new List<LocalizableText>();
                ControlsTexts[parent].Add(keyValuePair.Value);
                WaitingForRegister.Remove(keyValuePair.Key);
            }
        }

        //update all registered localizedtext with registering waiting before
        private static void UpdateLocalizedTextsForControl(Control control)
        {
            RegisterWaitingForRegister();

            List<LocalizableText> texts;

            if (!ControlsTexts.TryGetValue(control, out texts)) return;

            foreach (var localizableText in texts)
            {
                localizableText.UpdateTarget();
            }
        }

        //finds parent control wich locale specified
        private static Control FindDesignTimeLocaleParent(DependencyObject element)
        {
            var currentElement = element;
            do
            {
                if(currentElement is Control && DesignTimeLocales.ContainsKey(currentElement as Control))
                {
                    return currentElement as Control;
                }
            } while ((currentElement = VisualTreeHelper.GetParent(currentElement)) != null);

            return null;
        }
    }
}
