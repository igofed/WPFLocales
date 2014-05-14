using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Localization = WPFLocales.Localization;

namespace SampleApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Localization.Initialize(new DirectoryInfo("Locales"), "RU");
            base.OnStartup(e);
        }
    }
}
