using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPFLocales.DesignTime;
using WPFLocales.Utils;
using WPFLocales.View;

namespace WPFLocales
{
    public partial class Localization
    {
        /// <summary>
        /// Attached property for design time locale for current control
        /// </summary>
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
            if (!DesignerProperties.GetIsInDesignMode(dependencyObject))
                return;

            var control = dependencyObject as Control;
            if (control == null)
                return;

            //here need to check if locales loaded
            if (!_isLocalesLoaded)
            {
                Log("Initializing locales");

                var locales = GetAllLocales();
                var values = locales.ToDictionary(l => l.Key, l => l.Values);

                _designTimeLocaleValues = values;

                _isLocalesLoaded = true;

                Log("Initialized");
            }

            var newLocale = (string)dependencyPropertyChangedEventArgs.NewValue;
            //need to store locale for each control
            DesignTimeLocales[control] = newLocale;

            //on loading process we have no access to full XAML tree
            //so need check for IsLoaded and subscribe if need
            if (control.IsLoaded)
            {
                //if current control is loaded, then binding's converters's parents already setted
                //and we need only trigger bindings to update their targets
                dependencyObject.UpdateBindingTargets();
                //udpate all localized texts
                UpdateLocalizedTextsForControl(control);
            }
            else
            {
                control.Loaded += OnControlLoaded;
                control.Unloaded += OnControlUnloaded;
            }
        }

        private static void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            var control = sender as Control;
            if (control == null)
                return;

            //unregister from loaded event
            control.Loaded -= OnControlLoaded;

            //update LocalizableConverter parents for design time work
            control.UpdateBindingConverterParents();
            //update binding's targets for requesting new values
            control.UpdateBindingTargets();
            //udpate all localized texts
            UpdateLocalizedTextsForControl(control);
        }

        private static void OnControlUnloaded(object sender, RoutedEventArgs e)
        {
            var control = sender as Control;
            control.Unloaded -= OnControlUnloaded;

            DesignTimeLocales.Remove(control);
            ControlsTexts.Remove(control);
        }

        private static IEnumerable<DesignTimeLocale> GetAllLocales()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetCustomAttributes(typeof(LocalizationAssembly), false).Any());

            var types = from assembly in assemblies
                        from type in assembly.GetTypes()
                        where type.IsSubclassOf(typeof(DesignTimeLocale))
                        select (DesignTimeLocale)Activator.CreateInstance(type, null);

            return types;
        }

        //
        private static bool _isLocalesLoaded;
        //locale dictionaries for each locale parent
        private static IDictionary<string, IDictionary<string, IDictionary<string, string>>> _designTimeLocaleValues;
        //locale parent of each control, that uses localized strings
        private static readonly Dictionary<DependencyObject, Control> DesignTimeLocaleParents = new Dictionary<DependencyObject, Control>();
        //locales per each locale parent
        private static readonly Dictionary<Control, string> DesignTimeLocales = new Dictionary<Control, string>();
        //all localized texts of each control
        private static readonly Dictionary<Control, List<LocalizableText>> ControlsTexts = new Dictionary<Control, List<LocalizableText>>();
        //localized text waiting to be registered
        private static readonly Dictionary<DependencyObject, LocalizableText> WaitingForRegister = new Dictionary<DependencyObject, LocalizableText>();

        //returns localized text for element and localization key
        internal static string GetTextByLocalizationKey(DependencyObject element, Enum localicationKey)
        {
            //can occur for Converter which FormatKey was changed
            //just need reload designer for settings them back
            if (element == null)
                return "Please reload designer";

            //looking for element's parent with setted locale
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
            if (_designTimeLocaleValues.ContainsKey(locale))
            {
                var groups = _designTimeLocaleValues[locale];
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
                if (currentElement is Control && DesignTimeLocales.ContainsKey(currentElement as Control))
                {
                    return currentElement as Control;
                }
            } while ((currentElement = VisualTreeHelper.GetParent(currentElement)) != null);

            return null;
        }


        public static void Log(params string[] messages)
        {
            using (var writer = new StreamWriter("log.log", true))
            {
                foreach (var message in messages)
                {
                    writer.WriteLine(message);
                }
            }
        }
    }
}
