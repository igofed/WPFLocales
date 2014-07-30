using EnvDTE;
using PS.Properties;
using PS.Templates;
using PS.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace PS
{
    [Cmdlet(VerbsLifecycle.Enable, "Localization")]
    public class EnableLocalization : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, HelpMessage = "Project to store localization data in")]
        public string ProjectName { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Directory where localization data will be stored. Localization by default")]
        public string LocalizationDirectory { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "Directory where locale files will be stored. Locales by default")]
        public string LocalesDirectory { get; set; }


        protected override void ProcessRecord()
        {
            var dte = (DTE)GetVariableValue("DTE");

            //all projects in solution
            var projects = dte.Solution.GetProjects().ToArray();

            //test for existing localization project
            var localizationInfo = dte.Solution.GetLocalizationAssemblyInfo();
            if (localizationInfo != null)
            {
                Host.UI.WriteErrorLine(string.Format(@"Solution already contains localization project ""{0}""", localizationInfo.Project.Name));
                return;
            }

            //test for specified project exists
            var project = projects.FirstOrDefault(p => p.Name == ProjectName);
            if (project == null)
            {
                Host.UI.WriteErrorLine(string.Format(@"No project with name ""{0}"" available in solution", ProjectName));
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
                Host.UI.WriteErrorLine(string.Format(@"Locales and Localization directories should have different names"));
                return;
            }

            //add localization and locales directories
            project.AddDirectory(localesDirectoryName);
            var localizationDirectory = project.AddDirectory(localizationDirectoryName);
            localizationDirectory.AddDirectory(Resources.DesignTimeDataDirectoryName);

            //add localization keys file
            var localizationKeysFileText = GenerateLocalizationKeysFileText(project, localizationDirectoryName);
            project.AddFile(localizationDirectory, Resources.LocalizationKeysFileName, localizationKeysFileText);
                    
            //add localization assembly file
            var localizationAssemblyInfoFileText = GenerateLocalizationAssemblyInfoFileText(localesDirectoryName, localizationDirectoryName);
            var propertiesDirectory = project.GetPropertiesDirectory();
            project.AddFile(propertiesDirectory, Resources.LocalizationAssemblyInfoFileName, localizationAssemblyInfoFileText);
        }


        private static string GenerateLocalizationKeysFileText(Project project, string localizationDirectoryName)
        {
            var localizationKeysTemplate = new LocalizationKeysTemplate();
            localizationKeysTemplate.Session = new Dictionary<string, object>();
            localizationKeysTemplate.Session["NamespaceName"] = string.Format("{0}.{1}", project.GetRootNamespace(), localizationDirectoryName);
            localizationKeysTemplate.Initialize();
            return localizationKeysTemplate.TransformText();
        }

        private static string GenerateLocalizationAssemblyInfoFileText(string localesDirectoryName, string localizationDirectoryName)
        {
            var localizationAssemblyInfoTemplate = new LocalizationAssemblyInfoTemplate();
            localizationAssemblyInfoTemplate.Session = new Dictionary<string, object>();
            localizationAssemblyInfoTemplate.Session["LocalesDirectoryName"] = localesDirectoryName;
            localizationAssemblyInfoTemplate.Session["LocalizationDirectoryName"] = localizationDirectoryName;
            localizationAssemblyInfoTemplate.Initialize();
            return localizationAssemblyInfoTemplate.TransformText();
        }
    }
}
