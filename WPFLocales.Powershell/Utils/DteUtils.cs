using EnvDTE;
using PS.Properties;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PS.Utils
{
    internal static class DteUtils
    {
        public static IEnumerable<Project> GetProjects(this Solution solution)
        {
            return solution.Projects.Cast<Project>();
        }

        public static IEnumerable<ProjectItem> GetProjectItems(this Project project)
        {
            return project.ProjectItems.Cast<ProjectItem>();
        }

        public static IEnumerable<ProjectItem> GetProjectItemItems(this ProjectItem projectItem)
        {
            return projectItem.ProjectItems.Cast<ProjectItem>();
        }

        public static ProjectItem GetPropertiesDirectory(this Project project)
        {
            return project.GetProjectItems().FirstOrDefault(p => p.Name == Resources.PropertiesDirectoryName);
        }

        public static LocalizationAssemblyInfo GetLocalizationAssemblyInfo(this Project project)
        {
            return LocalizationAssemblyInfo.FromProject(project);
        }

        public static LocalizationAssemblyInfo GetLocalizationAssemblyInfo(this Solution solution)
        {
            return solution.GetProjects().Select(LocalizationAssemblyInfo.FromProject).FirstOrDefault(info => info != null);
        }

        public static ProjectItem AddFile(this Project project, ProjectItem parent, string fileName, string content)
        {
            var projectFileInfo = new FileInfo(project.FullName);
            var directory = projectFileInfo.Directory;
            var assemblyFileInfo = Path.Combine(directory.FullName, parent.Name, fileName);
            File.WriteAllText(assemblyFileInfo, content);

            return parent.ProjectItems.AddFromFile(assemblyFileInfo);
        }

        public static ProjectItem AddDirectory(this Project project, string directoryName)
        {
            return project.ProjectItems.AddFolder(directoryName);
        }

        public static ProjectItem AddDirectory(this ProjectItem projectItem, string directoryName)
        {
            return projectItem.ProjectItems.AddFolder(directoryName);
        }

        public static string GetRootNamespace(this Project project)
        {
            return project.GetPropertyValue<string>("RootNamespace");
        }

        private static T GetPropertyValue<T>(this Project project, string propertyName)
        {
            var property = project.Properties.Item(propertyName);

            if (property == null)
            {
                return default(T);
            }

            return (T)property.Value;
        }
    }
}
