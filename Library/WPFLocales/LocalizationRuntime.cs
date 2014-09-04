using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Xml.Serialization;
using WPFLocales.Exceptions;
using WPFLocales.Model;
using WpfLocales.Model.Xml;
using WPFLocales.Utils;

namespace WPFLocales
{
    /// <summary>
    /// Application's localization core
    /// </summary>
    public partial class Localization
    {
        /// <summary>
        /// Occurs on CurrentLocale changes
        /// </summary>
        public static event Action CurrentLocaleChanged = () => { };

        /// <summary>
        /// Gets available loaded locales
        /// </summary>
        public static IEnumerable<string> AvailableLocales { get; private set; }

        /// <summary>
        /// Gets default locale. Default locale used when items for current locale missing 
        /// </summary>
        public static string DefaultLocale { get; private set; }

        /// <summary>
        /// Gets or sets current locale, locale which is currently used by application
        /// </summary>
        public static string CurrentLocale
        {
            get { return _currentLocale; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    throw new NullReferenceException("Locale can't be null or empty");

                if (!AvailableLocales.Contains(value))
                    throw new NotSupportedException("Locale not specified in available locales");

                if (_currentLocale == value)
                    return;

                _currentLocale = value;

                CurrentLocaleChanged();

                //need update all bindings for LocalizableConverter work 
                Application.Current.UpdateBindingTargets();
            }
        }
        private static string _currentLocale;

        /// <summary>
        /// Gets if localization core initialized
        /// </summary>
        public static bool IsInitialized { get; private set; }

        /// <summary>
        /// ILocalization instance for injection
        /// </summary>
        public static ILocalization Instance
        {
            get
            {
                if (!IsInitialized)
                    throw new NotSupportedException("Initializa first");

                return _localizationInstance;
            }
        }
        private static ILocalization _localizationInstance;


        private static Dictionary<string, Dictionary<string, Dictionary<string, string>>> _allLocalesDictionary;


        /// <summary>
        /// Initializes application's localization core
        /// </summary>
        /// <param name="currentLocale">Current application locale - should be specified if start locale differs from default locale</param>
        public static void Initialize(string currentLocale = null)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetCustomAttributes(typeof(LocalesDirectory), false).Any()).ToArray();
            if (assemblies.Length > 1)
                throw new NotSupportedException(@"Two or more assemblies have ""LocalesDirectory"" attribute: " + string.Join(", ", assemblies.Select(a => a.FullName)));

            var assembly = assemblies[0];
            var localesDirectoryAttribute = (LocalesDirectory)assembly.GetCustomAttributes(typeof(LocalesDirectory), false).First();

            var localesDirectory = new DirectoryInfo(localesDirectoryAttribute.Value);

            if (!localesDirectory.Exists || !localesDirectory.GetFiles().Any(f => f.FullName.EndsWith(".locale")))
                throw new NotSupportedException("Directory should exists or contain locale files (.locale)");

            //read all locales from file
            var locales = new List<ILocale>();
            var files = localesDirectory.GetFiles().Where(f => f.FullName.EndsWith(".locale"));
            foreach (var file in files)
            {
                using (var stream = new FileStream(file.FullName, FileMode.Open))
                {
                    var serializer = new XmlSerializer(typeof(XmlLocale));
                    var locale = (XmlLocale)serializer.Deserialize(stream);
                    locales.Add(locale);
                }
            }

            //and initialize with locales
            Initialize(locales, currentLocale);
        }

        /// <summary>
        /// Initializes application's localization core
        /// </summary>
        /// <param name="locales">ILocale collection of locales</param>
        /// <param name="currentLocale">Current application locale - should be specified if start locale differs from default locale</param>
        public static void Initialize(IEnumerable<ILocale> locales, string currentLocale = null)
        {
            if (locales == null)
                throw new NullReferenceException("Locales should be specified");

            var localesList = locales.ToList();

            //search for locales with blank names
            if (localesList.Any(l => string.IsNullOrEmpty(l.Key)))
                throw new NotSupportedException("Locales with blank Key don't supported");

            //looking for duplicate locales
            var firstDuplicateLocale = localesList.GroupBy(l => l.Key).FirstOrDefault(g => g.Count() > 1);
            if (firstDuplicateLocale != null)
                throw new NotSupportedException(string.Format(@"Locale with Key ""{0}"" already specified", firstDuplicateLocale.Key));

            //looking for default locale in locales list
            if (!localesList.Any(l => l.IsDefault))
                throw new NotSupportedException("Locales should have default one");

            //check multi default locales
            if (localesList.Count(l => l.IsDefault) > 1)
                throw new NotSupportedException("Multiple default locales not supported");

            //looking for current locale in locales list
            if (!string.IsNullOrEmpty(currentLocale) && localesList.All(l => l.Key != currentLocale))
                throw new NotSupportedException(string.Format("Current locale ({0}) should be specified in locales", currentLocale));


            _allLocalesDictionary = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            foreach (var locale in localesList)
            {
                _allLocalesDictionary[locale.Key] = new Dictionary<string, Dictionary<string, string>>();

                //looking for empty group collection
                if (locale.Groups == null || locale.Groups.Count == 0)
                    throw new NotSupportedException(string.Format(@"Locale groups empty for locale ""{0}""", locale.Key));

                //looking for group with blank key
                if (locale.Groups.Any(g => string.IsNullOrEmpty(g.Key)))
                    throw new NotSupportedException(string.Format("Locale groups with blank Key don't supported. Locale {0}", locale.Key));

                //looking for duplicate groups
                var firstDuplicateGroup = locale.Groups.GroupBy(g => g.Key).FirstOrDefault(g => g.Count() > 1);
                if (firstDuplicateGroup != null)
                    throw new NotSupportedException(string.Format("Group with Key {0} already specified in locale {1}", firstDuplicateGroup.Key, locale.Key));

                foreach (var group in locale.Groups)
                {
                    //looking for items with blank key
                    if (group.Items.Any(i => string.IsNullOrEmpty(i.Key)))
                        throw new NotSupportedException(string.Format("Locale item with blank Key don't supported. Locale {0}, Group {1}", locale.Key, group.Key));

                    //looking for duplicate items
                    var firstDuplicateItems = group.Items.GroupBy(i => i.Key).FirstOrDefault(g => g.Count() > 1);
                    if (firstDuplicateItems != null)
                        throw new NotSupportedException(string.Format("Item with Key {0} already specified in group {1} in locale {2}", firstDuplicateItems.Key, group.Key, locale.Key));

                    //everything ok, just can add items to dictionary
                    _allLocalesDictionary[locale.Key][group.Key] = new Dictionary<string, string>();
                    foreach (var item in group.Items)
                    {
                        _allLocalesDictionary[locale.Key][group.Key][item.Key] = item.Value;
                    }
                }
            }

            AvailableLocales = new ReadOnlyCollection<string>(localesList.Select(l => l.Key).ToList());

            DefaultLocale = localesList.First(l => l.IsDefault).Key;
            CurrentLocale = string.IsNullOrEmpty(currentLocale) ? DefaultLocale : currentLocale;

            _localizationInstance = new LocalizationImplementation();

            //if we here, so all steps passed and since now Locales initialized
            IsInitialized = true;
        }

        /// <summary>
        /// Get localized string for localization key, if key not exists in current and default locale throws LocalizationKeyNotFoundException
        /// </summary>
        /// <param name="localizationKey">Localization key</param>
        /// <param name="isUseDefaultLocale">Specifies if default locale should be used if no item found in current, True by default</param>
        /// <returns>Localized string for specified localization key</returns>
        /// <exception cref="WPFLocales.Exceptions.LocalizationKeyNotFoundException">Throws if localization key not exists in current and default locale</exception>
        public static string GetTextByLocalizationKey(Enum localizationKey, bool isUseDefaultLocale = true)
        {
            string text;
            if (!TryGetTextByLocalizationKey(localizationKey, out text, isUseDefaultLocale))
            {
                throw new LocalizationKeyNotFoundException(string.Format("No item found for localization group {0} and item {1}", localizationKey.GetType().Name, localizationKey));
            }
            return text;
        }

        /// <summary>
        /// Get localized string for localization key. A return value indicates whether localized string was found.
        /// </summary>
        /// <param name="localizationKey">Localization key</param>
        /// <param name="text">Localized string for specified localization key</param>
        /// <param name="isUseDefaultLocale">Specifies if default locale should be used if no item found in current, True by default</param>
        /// <returns>Indicates whether localized string was found</returns>
        public static bool TryGetTextByLocalizationKey(Enum localizationKey, out string text, bool isUseDefaultLocale = true)
        {
            if (!IsInitialized)
                throw new NotSupportedException("Should be initialized before");

            if (localizationKey == null)
                throw new NullReferenceException("Key should be specified");

            //search for item in current locale
            var isFound = TryGetTextByKeyInLocaleDictionary(localizationKey, _allLocalesDictionary[CurrentLocale], out text);
            //if not found and default locale search turned and while current locale is not default - search for item in default locale 
            if (!isFound && isUseDefaultLocale && DefaultLocale != CurrentLocale)
            {
                isFound = TryGetTextByKeyInLocaleDictionary(localizationKey, _allLocalesDictionary[DefaultLocale], out text);
            }

            return isFound;
        }

        //search for item by key in localization dictionary
        private static bool TryGetTextByKeyInLocaleDictionary(Enum localizationKey, IDictionary<string, Dictionary<string, string>> localeDictionary, out string text)
        {
            var groupName = localizationKey.GetType().Name;
            var itemName = localizationKey.ToString();

            Dictionary<string, string> group;
            if (localeDictionary.TryGetValue(groupName, out group))
            {
                return group.TryGetValue(itemName, out text);
            }

            text = null;
            return false;
        }
    }
}
