using System.Collections.Generic;

namespace WPFLocales.Model
{
    public class Locale : ILocale
    {
        public string Key
        {
            get; set;
        }

        public string Title
        {
            get; set;
        }

        public bool IsDefault
        {
            get; set;
        }

        public IList<ILocaleGroup> Groups
        {
            get; set;
        }
    }
}
