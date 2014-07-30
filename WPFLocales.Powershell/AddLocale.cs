using System;
using EnvDTE;
using PS.Utils;
using System.Linq;
using System.Management.Automation;

namespace PS
{
    [Cmdlet(VerbsCommon.Add, "Locale")]
    public class AddLocale : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, HelpMessage = "Key of locale to add")]
        public string Key { get; set; }

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

            //todo: generate ok file with ok content
            localizationInfo.Project.AddFile(localizationInfo.LocalesDirectory, localeFileName, "");
        }
    }
}
