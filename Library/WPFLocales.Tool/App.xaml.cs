using System.Windows;
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

            var window = new MainWindow();
            window.DataContext = new MainViewModel();
            
            Current.MainWindow = window;
            Current.MainWindow.Show();
        }
    }
}
