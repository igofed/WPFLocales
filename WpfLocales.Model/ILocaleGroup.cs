using System.Collections.Generic;

namespace WPFLocales.Model
{
    public interface ILocaleGroup
    {
        string Key { get; }
        string Comment { get; }

        IList<ILocaleItem> Items { get; }
    }
}