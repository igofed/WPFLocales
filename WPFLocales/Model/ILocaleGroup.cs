using System.Collections.Generic;

namespace WPFLocales.Model
{
    public interface ILocaleGroup
    {
        string Key { get; set; }

        List<ILocaleItem> Items { get; set; }
    }
}