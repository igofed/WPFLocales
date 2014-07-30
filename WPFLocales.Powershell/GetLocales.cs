using EnvDTE;
using PS.Utils;
using System.Linq;
using System.Management.Automation;

namespace PS
{
    [Cmdlet(VerbsCommon.Get, "Locales")]
    public class GetLocales : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            var dte = (DTE)GetVariableValue("DTE");

            var localizationInfo = dte.Solution.GetLocalizationAssemblyInfo();
            if (localizationInfo == null)
            {
                Host.UI.WriteErrorLine("No localization project found. First do Enable-Localization and add some locales");
                return;
            }

            foreach (var locale in localizationInfo.LocalesDirectory.GetProjectItemItems().Where(i=>i.Name.EndsWith(".locale")))
            {
                Host.UI.WriteLine(locale.Name);
            }  
        }
    }
}
