using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using WpfLocales.Model.Xml;
using WPFLocales;
using WPFLocales.Model;
using WPFLocales.Powershell.Templates;

namespace SampleApplication
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            using (var reader = new StreamReader(@"Locales\RU.locale"))
            {
                var s = new XmlSerializer(typeof(XmlLocale));
                var locale = (ILocale)s.Deserialize(reader);

                var template = new LocalizationKeysTemplate();
                template.Session = new Dictionary<string, object>();
                template.Session["NamespaceName"] = "RRrr";
                template.Session["Locale"] = locale;
                template.Initialize();
                var text = template.TransformText();
            }

            Locales.Initialize(new DirectoryInfo("Locales"), "RU");
            base.OnStartup(e);
        }
    }
}
