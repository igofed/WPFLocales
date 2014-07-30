using EnvDTE;
using PS.Templates;
using PS.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace PS
{
    [Cmdlet(VerbsCommon.Add, "Locale")]
    public class AddLocale : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, HelpMessage = "Key of locale to add")]
        public string Key { get; set; }

        [Parameter(Mandatory = true, Position = 1, HelpMessage = "Title of locale to add")]
        public string Title { get; set; }

        protected override void ProcessRecord()
        {
            var dte = (DTE)GetVariableValue("DTE");

            var localizationInfo = dte.Solution.GetLocalizationAssemblyInfo();
            if (localizationInfo == null)
            {
                Host.UI.WriteErrorLine("No localization project found. First do Enable-Localization");
                return;
            }
            if (localizationInfo.LocalesDirectory == null || localizationInfo.LocalizationDirectory == null)
            {
                Host.UI.WriteErrorLine("Something wrong with localization settings");
                return;
            }


            var localeFileName = string.Format("{0}.locale", Key);

            var existingLocale = localizationInfo.LocalesDirectory.GetProjectItemItems().FirstOrDefault(i => i.Name == localeFileName);
            if (existingLocale != null)
            {
                Host.UI.WriteErrorLine("Locale with such key already exists");
                return;
            }

            var localeFileText = GenerateLocaleFileText(Key, Title);
            var localeFile = localizationInfo.Project.AddFile(localizationInfo.LocalesDirectory, localeFileName, localeFileText);
            
            foreach (var VARIABLE in localeFile.Properties)
            {
                
                var prop = (Property) VARIABLE;
                Host.UI.WriteLine(string.Format("{0}->{1}", prop.Name, prop.Value));
                
            }
        }

        private string GenerateLocaleFileText(string key, string locale)
        {
            var localeTemplate = new LocaleTemplate();
            localeTemplate.Session = new Dictionary<string, object>();
            localeTemplate.Session["Key"] = key;
            localeTemplate.Session["Title"] = Title;
            localeTemplate.Initialize();
            return localeTemplate.TransformText();
        }
    }
}
