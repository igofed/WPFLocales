using System.Collections.Generic;
using WPFLocales.Model;

namespace WPFLocales.Powershell.Templates
{
    internal class TemplatesHelper
    {
        public static string GenerateLocalizationKeysFileText(string rootNameSpace, string localizationDirectoryName, ILocale locale = null)
        {
            var localizationKeysTemplate = new LocalizationKeysTemplate();
            localizationKeysTemplate.Session = new Dictionary<string, object>();
            localizationKeysTemplate.Session["NamespaceName"] = string.Format("{0}.{1}", rootNameSpace, localizationDirectoryName);
            localizationKeysTemplate.Session["Locale"] = locale;
            localizationKeysTemplate.Initialize();
            return localizationKeysTemplate.TransformText();
        }

        public static string GenerateLocalizationAssemblyInfoFileText(string localesDirectoryName, string localizationDirectoryName)
        {
            var localizationAssemblyInfoTemplate = new LocalizationAssemblyInfoTemplate();
            localizationAssemblyInfoTemplate.Session = new Dictionary<string, object>();
            localizationAssemblyInfoTemplate.Session["LocalesDirectoryName"] = localesDirectoryName;
            localizationAssemblyInfoTemplate.Session["LocalizationDirectoryName"] = localizationDirectoryName;
            localizationAssemblyInfoTemplate.Initialize();
            return localizationAssemblyInfoTemplate.TransformText();
        }

        public static string GenerateLocaleFileText(ILocale locale)
        {
            var localeTemplate = new LocaleTemplate();
            localeTemplate.Session = new Dictionary<string, object>();
            localeTemplate.Session["Locale"] = locale;
            localeTemplate.Initialize();
            return localeTemplate.TransformText();
        }

        public static string GenerateDesignTimeLocaleFileText(string key, string title, string rootNamespace, string localizationNamespace, string designDataNamespace, ILocale locale)
        {
            var localeDesignTemplate = new DesignTimeLocaleTemplate();
            localeDesignTemplate.Session = new Dictionary<string, object>();
            localeDesignTemplate.Session["Key"] = key;
            localeDesignTemplate.Session["Title"] = title;
            localeDesignTemplate.Session["NamespaceName"] = string.Format("{0}.{1}.{2}", rootNamespace, localizationNamespace, designDataNamespace);
            localeDesignTemplate.Session["Locale"] = locale;
            localeDesignTemplate.Initialize();
            return localeDesignTemplate.TransformText();
        }
    }
}
