using EnvDTE;
using WPFLocales.Model;

namespace WPFLocales.Powershell.Utils
{
    public class LocaleInfo
    {
        public ILocale Locale { get; set; }
        public ProjectItem LocaleItem { get; set; }
        public ProjectItem DesignTimeLocaleItem { get; set; }
    }
}
