using System.Collections.Generic;

namespace WPFLocales.Model
{
    public interface ILocale
    {
        string Key { get; }
        string Title { get; }
        bool IsDefault { get; }

        IList<ILocaleGroup> Groups { get; }
    }
}