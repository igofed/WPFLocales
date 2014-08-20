using EnvDTE;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Xml.Serialization;
using WpfLocales.Model.Xml;
using WPFLocales.Model;
using WPFLocales.Powershell.Properties;
using WPFLocales.Powershell.Utils;

namespace WPFLocales.Powershell
{
    public abstract class LocalizationCmdlet : PSCmdlet
    {
        protected DTE _dte;
        protected LocalizationAssemblyInfo _localizationInfo;


        protected override void BeginProcessing()
        {
            _dte = (DTE)GetVariableValue("DTE");

            FindLocalizationInfo();
        }


        protected void WriteErrorLine(string line)
        {
            Host.UI.WriteErrorLine(line);
        }

        protected void WriteLine(string line)
        {
            Host.UI.WriteLine(line);
        }

        protected void FindLocalizationInfo()
        {
            _localizationInfo = _dte.Solution.GetLocalizationAssemblyInfo();
        }

        protected LocaleInfo[] GetAlLocales()
        {
            var locales = new List<LocaleInfo>();

            //get all locale items
            foreach (var locale in _localizationInfo.LocalesDirectory.GetProjectItemItems().Where(i => i.Name.EndsWith(".locale")))
            {
                var fullPath = locale.GetFullPath();

                //get design time locale item
                var name = Path.GetFileNameWithoutExtension(fullPath);
                var designTimeLocaleFileName = string.Format("{0}DesignTimeLocale.cs", name);
                var designTimeLocaleItem = _localizationInfo.LocalizationDesignDataDirectory.GetProjectItemItems().FirstOrDefault(i => i.Name == designTimeLocaleFileName);

                //todo: throw if file missing /\/\/\/\

                using (var reader = new StreamReader(fullPath))
                {
                    var serializer = new XmlSerializer(typeof(XmlLocale));
                    var localeObject = (ILocale)serializer.Deserialize(reader);

                    locales.Add(new LocaleInfo { Locale = localeObject, LocaleItem = locale, DesignTimeLocaleItem = designTimeLocaleItem});
                }
            }

            return locales.ToArray();
        }

        protected LocaleInfo GetDefaultLocale()
        {
            return GetAlLocales().FirstOrDefault(l => l.Locale.IsDefault);
        }

        protected void AddLocale(string localeKey, string localeTitle, ILocale defaultLocale = null)
        {
            //create lcocale file name
            var localeFileName = string.Format("{0}.locale", localeKey);

            //test for such locale exist
            var existingLocale = _localizationInfo.LocalesDirectory.GetProjectItemItems().FirstOrDefault(i => i.Name == localeFileName);
            if (existingLocale != null)
            {
                WriteErrorLine("Locale with such key already exists");
                return;
            }

            //create locale file
            var localeFileText = Templates.TemplatesHelper.GenerateLocaleFileText(localeKey, localeTitle, defaultLocale);
            var localeFile = _localizationInfo.LocalesDirectory.AddFile(localeFileName, localeFileText);
            WriteLine("Locale file created");


            //create design time locale file
            var designTimeLocaleFileName = string.Format("{0}DesignTimeLocale.cs", localeKey);
            var designTimeLocaleFileText = Templates.TemplatesHelper.GenerateDesignTimeLocaleFileText(localeKey, localeTitle, _localizationInfo.Project.GetRootNamespace(), _localizationInfo.LocalizationNamespace, Resources.DesignTimeDataDirectoryName);
            _localizationInfo.LocalizationDesignDataDirectory.AddFile(designTimeLocaleFileName, designTimeLocaleFileText);
            WriteLine("Design time locale created");

            var fullPath = localeFile.GetFullPath();
            _dte.ItemOperations.OpenFile(fullPath);

            WriteLine("Locale added");
        }

        protected bool IsLocaleKeyValid(string localeName)
        {
            return localeName.Length >= 2 && localeName.All(char.IsLetter);
        }
    }
}
