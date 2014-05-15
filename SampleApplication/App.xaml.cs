using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WPFLocales;

namespace SampleApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Locales.Initialize(new DirectoryInfo("Locales"), "RU");
            base.OnStartup(e);
        }
    }
}
