using System.Linq;
using System.Management.Automation;
using WPFLocales.Powershell.Properties;
using WPFLocales.Powershell.Utils;

namespace WPFLocales.Powershell
{
    [Cmdlet(VerbsLifecycle.Enable, "Localization")]
    public class EnableLocalization : LocalizationCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "Project to store localization data in")]
        public string ProjectName { get; set; }

        [Parameter(Mandatory = true, HelpMessage = "Key of application's default locale")]
        public string DefaultLocaleKey { get; set; }

        [Parameter(Mandatory = true, HelpMessage = "Title of application's default locale")]
        public string DefaultLocaleTitle { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Directory where localization data will be stored. Localization by default")]
        public string LocalizationDirectory { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Directory where locale files will be stored. Locales by default")]
        public string LocalesDirectory { get; set; }


        protected override void ProcessRecord()
        {
            //test for existing localization project
            if (_localizationInfo != null)
            {
                WriteErrorLine(string.Format(@"Solution already contains localization project ""{0}""", _localizationInfo.Project.Name));
                return;
            }

            //test for specified project exists
            var project = _dte.Solution.GetProjects().FirstOrDefault(p => p.Name == ProjectName);
            if (project == null)
            {
                WriteErrorLine(string.Format(@"No project with name ""{0}"" available in solution", ProjectName));
                return;
            }

            if (!IsLocaleKeyValid(DefaultLocaleKey))
            {
                WriteErrorLine("Locale key shoud has length of minimum 2 and contains only chars");
                return;
            }

            //check for localization and locales directory names
            var localesDirectoryName = Resources.LocalesDirectoryDefaultName;
            var localizationDirectoryName = Resources.LocalizationDirectoryDefaultName;
            if (!string.IsNullOrEmpty(LocalesDirectory))
                localesDirectoryName = LocalesDirectory;
            if (!string.IsNullOrEmpty(LocalizationDirectory))
                localizationDirectoryName = LocalizationDirectory;

            if (localizationDirectoryName == localesDirectoryName)
            {
                WriteErrorLine(string.Format(@"Locales and Localization directories should have different names"));
                return;
            }

            //add localization and locales directories
            project.AddDirectory(localesDirectoryName);
            var localizationDirectory = project.AddDirectory(localizationDirectoryName);
            localizationDirectory.AddDirectory(Resources.DesignTimeDataDirectoryName);
            WriteLine("Localization and locales directories created");


            //add localization keys file
            var localizationKeysFileText = Templates.Templates.GenerateLocalizationKeysFileText(project.GetRootNamespace(), localizationDirectoryName);
            localizationDirectory.AddFile(Resources.LocalizationKeysFileName, localizationKeysFileText);
            WriteLine("Localization keys file created");

            //add localization assembly file
            var localizationAssemblyInfoFileText = Templates.Templates.GenerateLocalizationAssemblyInfoFileText(localesDirectoryName, localizationDirectoryName);
            var propertiesDirectory = project.GetPropertiesDirectory();
            propertiesDirectory.AddFile(Resources.LocalizationAssemblyInfoFileName, localizationAssemblyInfoFileText);
            WriteLine("Localization assembly file created");

            //now localization enabled - request localization info
            FindLocalizationInfo();

            //add default locale
            AddLocale(DefaultLocaleKey, DefaultLocaleTitle, GetDefaultLocale().Locale);
           
            WriteLine("Localization enabled");
        }
    }
}
