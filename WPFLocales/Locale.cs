using System.Collections.Generic;

namespace WPFLocales
{
    public abstract class Locale
    {
        /// <summary>
        /// Key of locale
        /// </summary>
        public string Key { get; private set; }
        /// <summary>
        /// Locale's string values
        /// </summary>
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Values { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        protected Locale(string key)
        {
            Key = key;
        }
    }
}
