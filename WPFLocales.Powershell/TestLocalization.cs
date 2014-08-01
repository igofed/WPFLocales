using System.Management.Automation;

namespace WPFLocales.Powershell
{
    [Cmdlet(VerbsDiagnostic.Test, "Localization")]
    public class TestLocalization : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            base.ProcessRecord();
        }
    }
}
