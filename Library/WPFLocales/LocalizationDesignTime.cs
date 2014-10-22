using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
            
            control.Unloaded += OnControlUnloaded;

            //here need to check if locales loaded
            if (_designTimeLocaleValues == null || _designTimeLocaleValues.Count == 0)
            {
                var locales = GetAllLocales().ToArray();
                var values = locales.ToDictionary(l => l.Key, l => l.Values);
                _designTimeLocaleValues = values;
            }

            var newLocale = (string)dependencyPropertyChangedEventArgs.NewValue;
            //need to store locale for each control
            DesignTimeLocales[control] = newLocale;

            //on loading process we have no access to full XAML tree
            //so need check for IsLoaded and subscribe if need
            if (control.IsLoaded)
            {
                //find all registered localizable texts for control and set their's locale parent
                FindRegisteredLocalizableTextParents(control);
                //udpate all localized texts
                UpdateLocalizedTextsForControl(control);

                //update LocalizableConverter parents for design time work
                control.UpdateBindingConverterParents();
                //update binding's targets for requesting new values
                control.UpdateBindingTargets();
            }
            else
            {
                control.Loaded += OnControlLoaded;
            }
        }

        private static void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            var control = sender as Control;
            if (control == null)
                return;

            //unregister from loaded event
            control.Loaded -= OnControlLoaded;

            //find all registered localizable texts for control and set their's locale parent
            FindRegisteredLocalizableTextParents(control);
            //udpate all localized texts
            UpdateLocalizedTextsForControl(control);

            //update LocalizableConverter parents for design time work
            control.UpdateBindingConverterParents();
            //update binding's targets for requesting new values
            control.UpdateBindingTargets();
        }

        private static void OnControlUnloaded(object sender, RoutedEventArgs e)
        {
            var control = sender as Control;
            if (control == null)
                return;

            control.Unloaded -= OnControlUnloaded;

            DesignTimeLocales.Remove(control);
            ControlsTexts.Remove(control);
        }

        //returns all classes derived from DesignTimeLocale in library marked with LocalizationAssembly attribute
        private static IEnumerable<DesignTimeLocale> GetAllLocales()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetCustomAttributes(typeof(LocalizationAssembly), false).Any());

            var types = from assembly in assemblies
                        from type in assembly.GetTypes()
                        where type.IsSubclassOf(typeof(DesignTimeLocale))
                        select (DesignTimeLocale)Activator.CreateInstance(type, null);

            return types;
        }


        //locale dictionaries for each locale parent
        private static IDictionary<string, IDictionary<string, IDictionary<string, string>>> _designTimeLocaleValues;
        //locales per each locale parent
        private static readonly Dictionary<Control, string> DesignTimeLocales = new Dictionary<Control, string>();

        //all localized texts of each control
        private static readonly Dictionary<Control, List<LocalizableText>> ControlsTexts = new Dictionary<Control, List<LocalizableText>>();
        //localized text waiting to be registered
        private static readonly Dictionary<DependencyObject, LocalizableText> RegisteredLocalizableTexts = new Dictionary<DependencyObject, LocalizableText>();


        //returns localized text for element and localization key
        internal static string GetTextByLocalizationKey(Control localeParent, Enum localicationKey)
        {
            if (localeParent == null)
                return "No design time locale specified";

            var locale = DesignTimeLocales[localeParent];
            var groupKey = localicationKey.GetType().Name;
            var itemKey = localicationKey.ToString();

            //looking for item in dictionary
            var item = "No such locale available";
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


        //registers LocalizableText to waiting collection for processing after parent control updated
        internal static void RegisterLocalizableTextForElement(LocalizableText localizableText, DependencyObject element)
        {
            RegisteredLocalizableTexts.Add(element, localizableText);
        }

        //registers all waiting controls 
        private static void FindRegisteredLocalizableTextParents(Control control)
        {
            if (!RegisteredLocalizableTexts.Any())
                return;

            ControlsTexts[control] = new List<LocalizableText>();

            foreach (var element in control.EnumerateVisualChildrenRecoursively().Where(e => RegisteredLocalizableTexts.ContainsKey(e)))
            {
                var localizableText = RegisteredLocalizableTexts[element];
                localizableText.DesignLocaleParent = control;
                ControlsTexts[control].Add(localizableText);

                RegisteredLocalizableTexts.Remove(element);
            }
        }

        //update all registered localizedtext with registering waiting before
        private static void UpdateLocalizedTextsForControl(Control control)
        {
            List<LocalizableText> texts;
            if (!ControlsTexts.TryGetValue(control, out texts)) return;

            foreach (var localizableText in texts)
            {
                localizableText.UpdateTarget();
            }
        }
    }
}
