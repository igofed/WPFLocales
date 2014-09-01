﻿using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using WpfLocales.Model.Xml;
using WPFLocales.Model;
using WPFLocales.Powershell.Templates;
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
            using (var reader = new StreamReader(@"Locales\RU.locale"))
            {
                var s = new XmlSerializer(typeof(XmlLocale));
                var locale = (ILocale)s.Deserialize(reader);

                var template = new LocaleTemplate();
                template.Session = new Dictionary<string, object>();
                template.Session["Locale"] = locale;
                template.Initialize();
                var text = template.TransformText();
            }

            Localization.Initialize();
            base.OnStartup(e);
        }
    }
}