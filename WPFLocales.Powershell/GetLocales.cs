using System.Linq;
using System.Management.Automation;

namespace WPFLocales.Powershell
{
    [Cmdlet(VerbsCommon.Get, "Locales")]
    public class GetLocales : LocalizationCmdlet
    {
        protected override void ProcessRecord()
        {
            if (_localizationInfo == null)
            {
                Host.UI.WriteErrorLine("No localization project found. First do Enable-Localization and add some locales");
                return;
            }

            var defaultLocale = GetDefaultLocale();
            WriteLine(string.Format("{0} -> {1} -> {2} (Default)", defaultLocale.LocaleItem.Name, defaultLocale.Locale.Key, defaultLocale.Locale.Title));

            foreach (var locale in GetAlLocales().Where(l => l.Locale.Key != defaultLocale.Locale.Key))
            {
                WriteLine(string.Format("{0} -> {1} -> {2}", locale.LocaleItem.Name, locale.Locale.Key, locale.Locale.Title));
            }
        }
    }
}
