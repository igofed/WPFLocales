using System;

namespace WPFLocales
{
    public interface ILocalization
    {
        string GetTextByLocalizationKey(Enum localizationKey, bool isUseDefaultLocale = true);
        bool TryGetTextByLocalizationKey(Enum localizationKey, out string text, bool isUseDefaultLocale = true);
    }
}
