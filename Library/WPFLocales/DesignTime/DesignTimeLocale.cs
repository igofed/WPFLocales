using System.Collections.Generic;

namespace WPFLocales.DesignTime
{
    /// <summary>
    /// Class for locales in design time
    /// </summary>
    public abstract class DesignTimeLocale
    {
        /// <summary>
        /// Key of locale
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Title of locale
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Locale's string values
        /// </summary>
        public IDictionary<string, IDictionary<string, string>> Values { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        protected DesignTimeLocale(string key, string title)
        {
            Key = key;
            Title = title;
        }
    }
}
