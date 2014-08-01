using System.Management.Automation;
using WPFLocales.Powershell.Properties;
using WPFLocales.Powershell.Utils;

namespace WPFLocales.Powershell
{
    [Cmdlet(VerbsData.Sync, "Localization")]
    public class SyncLocalization : LocalizationCmdlet
    {
        protected override void ProcessRecord()
        {
            //check for localization enabled
            if (_localizationInfo == null)
            {
                WriteErrorLine("No localization project found. First do Enable-Localization");
                return;
            }

            //sync localekeys file
            var defaultLocale = GetDefaultLocale();
            if (defaultLocale == null)
            {
                WriteErrorLine("No default locale found");
                return;
            }

            var localizationKeysFileText = Templates.Templates.GenerateLocalizationKeysFileText(_localizationInfo.Project.GetRootNamespace(), _localizationInfo.LocalizationNamespace, defaultLocale.Locale);
            _localizationInfo.LocalizationKeys.ChangeContent(localizationKeysFileText);

            //sync designTimeLocale file
            foreach (var localeInfo in GetAlLocales())
            {
                var designTimeLocaleFileText = Templates.Templates.GenerateDesignTimeLocaleFileText(localeInfo.Locale.Key, localeInfo.Locale.Title, _localizationInfo.Project.GetRootNamespace(), _localizationInfo.LocalizationNamespace, Resources.DesignTimeDataDirectoryName, localeInfo.Locale);
                localeInfo.DesignTimeLocaleItem.ChangeContent(designTimeLocaleFileText);
                WriteLine(string.Format(@"Design time locale for ""{0}"" synced", localeInfo.Locale.Key));
            }
        }
    }
}
