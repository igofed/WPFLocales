using System.IO;
using System.Windows;
using WPFLocales.Tool.Models;
using WPFLocales.Tool.ViewModels.Common;

namespace WPFLocales.Tool.ViewModels.Config
{
    internal class ConfigModeEditViewModel : ConfigModeViewModel
    {
        public string DefaultLocalePath
        {
            get { return _defaultLocalePath; }
            set { if (Set(ref _defaultLocalePath, value, () => DefaultLocalePath)) EditCommand.RaiseCanExecuteChanged(); }
        }

        public DelegateCommand EditCommand { get; set; }
        public DelegateCommand DefaultLocaleFindCommand { get; set; }

        public LocaleContainer DefauleLocale { get; set; }


        private string _defaultLocalePath;


        public ConfigModeEditViewModel()
        {
            InitCommands();
        }


        private void InitCommands()
        {
            DefaultLocaleFindCommand = new DelegateCommand(DefaultLocaleFindCommandExecute);
            EditCommand = new DelegateCommand(EditCommandExecute, CanEditCommandExecute);
        }

        private void EditCommandExecute()
        {
            var defaultLocale = LocaleContainer.ReadFromFile(DefaultLocalePath);
            if (!defaultLocale.Locale.IsDefault)
            {
                MessageBox.Show("Locale is not marked as default", "Default locale error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DefauleLocale = defaultLocale;

            RaiseCompleted();
        }
        private bool CanEditCommandExecute()
        {
            return File.Exists(DefaultLocalePath) && Path.GetExtension(DefaultLocalePath) == ".locale";
        }

        private void DefaultLocaleFindCommandExecute()
        {
            var path = FileDialogUtils.FindLocaleFile();
            if (string.IsNullOrEmpty(path))
                return;

            var defaultLocaleContainer = LocaleContainer.ReadFromFile(path);
            if (!defaultLocaleContainer.Locale.IsDefault)
            {
                MessageBox.Show("Locale is not marked as default", "Default locale error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DefaultLocalePath = path;
        }
    }
}