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

            foreach (var locale in GetAlLocales())
            {
                WriteLine(string.Format("{0} -> {1} -> {2}", locale.LocaleItem.Name, locale.Locale.Key, locale.Locale.Title));
            }
        }
    }
}
