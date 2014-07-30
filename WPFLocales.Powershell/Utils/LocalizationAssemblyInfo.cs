using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using EnvDTE;
using PS.Properties;

namespace PS.Utils
{
    internal class LocalizationAssemblyInfo
    {
        public Project Project { get; set; }
        public ProjectItem LocalesDirectory { get; set; }
        public ProjectItem LocalizationDirectory { get; set; }

        public static LocalizationAssemblyInfo FromProject(Project project)
        {
            var propertiesDirectory = project.GetPropertiesDirectory();
            if (propertiesDirectory == null)
                return null;

            var assemblyItem = propertiesDirectory.GetProjectItemItems().FirstOrDefault(f => f.Name == Resources.LocalizationAssemblyInfoFileName);
            if (assemblyItem == null)
                return null;

            var assemblyItemPath = assemblyItem.FileNames[0];
            var assemblyItemText = File.ReadAllText(assemblyItemPath);

            var localizationDirectoryName = GetLocalizationAssemblyAttributeValue(assemblyItemText, Resources.LocalizationDirectoryAttributeName);
            var localesDirectoryName = GetLocalizationAssemblyAttributeValue(assemblyItemText, Resources.LocalesDirectoryAttributeName);

            var localesDirectory = project.GetProjectItems().FirstOrDefault(i => i.Name == localesDirectoryName);
            var localizationDirectory = project.GetProjectItems().FirstOrDefault(i => i.Name == localizationDirectoryName);

            return new LocalizationAssemblyInfo
            {
                Project =  project,
                LocalesDirectory = localesDirectory,
                LocalizationDirectory = localizationDirectory
            };
        }

        private static string GetLocalizationAssemblyAttributeValue(string input, string attributeName)
        {
            var regex = string.Format(@"\[assembly: {0}\(""(?<value>[\w\d]+)""\)\]", attributeName);
            var match = Regex.Match(input, regex);
            return match.Groups["value"].Value;
        }
    }
}
