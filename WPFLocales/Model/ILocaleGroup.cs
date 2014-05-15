using System.Collections.Generic;

namespace WPFLocales.Model
{
    public interface ILocaleGroup
    {
        string Key { get; }

        List<ILocaleItem> Items { get; }
    }
}