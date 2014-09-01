using EnvDTE;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WPFLocales.Powershell.Properties;

namespace WPFLocales.Powershell.Utils
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

        public static ProjectItem AddFile(this Project project, string fileName, string content)
        {
            var projectFileInfo = new FileInfo(project.FullName);
            var directory = projectFileInfo.Directory;
            var filePath = Path.Combine(directory.FullName, fileName);
            File.WriteAllText(filePath, content);

            return project.ProjectItems.AddFromFile(filePath);
        }

        public static ProjectItem AddFile(this ProjectItem projectItem, string fileName, string content)
        {
            var directory = projectItem.GetFullPath();
            var assemblyFileInfo = Path.Combine(directory, fileName);
            File.WriteAllText(assemblyFileInfo, content);

            return projectItem.ProjectItems.AddFromFile(assemblyFileInfo);
        }

        public static ProjectItem AddDirectory(this Project project, string directoryName)
        {
            return project.ProjectItems.AddFolder(directoryName);
        }

        public static ProjectItem AddDirectory(this ProjectItem projectItem, string directoryName)
        {
            return projectItem.ProjectItems.AddFolder(directoryName);
        }

        public static void ChangeContent(this ProjectItem projectItem, string newContent)
        {
            var fullPath = projectItem.GetFullPath();
            File.WriteAllText(fullPath, newContent);
        }

        public static string GetRootNamespace(this Project project)
        {
            return project.GetPropertyValue<string>("RootNamespace");
        }

        public static string GetFullPath(this ProjectItem item)
        {
            return item.GetPropertyValue<string>("FullPath");
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

        private static T GetPropertyValue<T>(this ProjectItem item, string propertyName)
        {
            var property = item.Properties.Item(propertyName);

            if (property == null)
            {
                return default(T);
            }

            return (T)property.Value;
        }
    }
}
