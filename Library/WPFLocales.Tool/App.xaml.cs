using System;
using System.IO;
using System.Windows;
using System.Xml.Serialization;
using WpfLocales.Model.Xml;
using WPFLocales.Tool.Models;
using WPFLocales.Tool.ViewModels;
using WPFLocales.Tool.Views;

namespace WPFLocales.Tool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (e.Args.Length > 2)
            {
                MessageBox.Show("Maximum 2 locales allowed in args", "Start error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            LocaleContainer defaultLocale = null;
            LocaleContainer newLocale = null;
            try
            {
                if (e.Args.Length >= 1)
                    defaultLocale = LocaleContainer.ReadFromFile(e.Args[0]); 
                if (e.Args.Length == 2)
                    newLocale = LocaleContainer.ReadFromFile(e.Args[1]); 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Start error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var window = new MainWindow();
            window.DataContext = new MainViewModel(defaultLocale, newLocale);

            Current.MainWindow = window;
            Current.MainWindow.Show();
        }
    }
}
