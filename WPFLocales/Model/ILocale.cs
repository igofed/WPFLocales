using System.Collections.Generic;

namespace WPFLocales.Model
{
    public interface ILocale
    {
        string Key { get; set; }

        List<ILocaleGroup> Groups { get; set; }
    }
}