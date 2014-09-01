using System.Collections.Generic;

namespace WPFLocales.Model
{
    public class LocaleGroup : ILocaleGroup
    {
        public string Key
        {
            get; set;
        }

        public string Comment
        {
            get; set;
        }

        public IList<ILocaleItem> Items
        {
            get; set;
        }
    }
}
