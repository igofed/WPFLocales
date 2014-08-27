using System;

namespace WPFLocales
{
    internal class LocalizationImplementation : ILocalization
    {
        public string GetTextByLocalizationKey(Enum localizationKey, bool isUseDefaultLocale = true)
        {
            return Localization.GetTextByLocalizationKey(localizationKey, isUseDefaultLocale);
        }

        public bool TryGetTextByLocalizationKey(Enum localizationKey, out string text, bool isUseDefaultLocale = true)
        {
            return Localization.TryGetTextByLocalizationKey(localizationKey, out text, isUseDefaultLocale);
        }
    }
}
