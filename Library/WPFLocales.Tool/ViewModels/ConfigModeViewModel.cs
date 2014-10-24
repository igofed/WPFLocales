using System;
using System.IO;
using System.Windows;
using WPFLocales.Tool.Models;
using WPFLocales.Tool.ViewModels.Common;

namespace WPFLocales.Tool.ViewModels
{
    internal class ConfigModeViewModel : ViewModelBase
    {
        public event Action Completed = () => { };

        protected void RaiseCompleted()
        {
            Completed();
        }
    }

    internal class ConfigModeTranslateViewModel : ConfigModeViewModel
    {
        public string DefaultLocalePath
        {
            get { return _defaultLocalePath; }
            set { if (Set(ref _defaultLocalePath, value, () => DefaultLocalePath)) TranslateCommand.RaiseCanExecuteChanged(); }
        }
        public string NewLocalePath
        {
            get { return _newLocalePath; }
            set { if (Set(ref _newLocalePath, value, () => NewLocalePath)) TranslateCommand.RaiseCanExecuteChanged(); }
        }

        public DelegateCommand TranslateCommand { get; set; }
        public DelegateCommand DefaultLocaleFindCommand { get; set; }
        public DelegateCommand NewLocaleFindCommand { get; set; }
        public DelegateCommand NewLocaleCreateCommand { get; set; }

        public LocaleContainer DefauleLocale { get; set; }
        public LocaleContainer NewLocale { get; set; }

        private string _defaultLocalePath;
        private string _newLocalePath;


        public ConfigModeTranslateViewModel()
        {
            InitCommands();
        }


        private void InitCommands()
        {
            TranslateCommand = new DelegateCommand(TranslateCommandExecute, CanTranslateCommandExecute);
            DefaultLocaleFindCommand = new DelegateCommand(DefaultLocaleFindCommandExecute);
            NewLocaleFindCommand = new DelegateCommand(NewLocaleFindCommandExecute);
            NewLocaleCreateCommand = new DelegateCommand(NewLocaleCreateCommandExecute);
        }


        private void TranslateCommandExecute()
        {
            var defaultLocale = LocaleContainer.ReadFromFile(DefaultLocalePath);
            if (!defaultLocale.Locale.IsDefault)
            {
                MessageBox.Show("Default locale is not marked as default", "Default locale error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var newLocale = LocaleContainer.ReadFromFile(NewLocalePath);
            if (newLocale.Locale.IsDefault)
            {
                MessageBox.Show("New locale marked as default", "New locale error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DefauleLocale = defaultLocale;
            NewLocale = newLocale;

            RaiseCompleted();
        }
        private bool CanTranslateCommandExecute()
        {
            return File.Exists(DefaultLocalePath) && File.Exists(NewLocalePath)
                && Path.GetExtension(DefaultLocalePath) == ".locale" && Path.GetExtension(NewLocalePath) == ".locale";
        }

        private void DefaultLocaleFindCommandExecute()
        {
            var path = FileDialogUtils.FindLocaleFile();
            if (path == NewLocalePath)
            {
                MessageBox.Show("Default and new locale shoud be different", "Locale selecting error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            DefaultLocalePath = path;
        }

        private void NewLocaleFindCommandExecute()
        {
            var path = FileDialogUtils.FindLocaleFile();
            if (path == DefaultLocalePath)
            {
                MessageBox.Show("Default and new locale shoud be different", "Locale selecting error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            NewLocalePath = path;
        }

        private void NewLocaleCreateCommandExecute()
        {
            NewLocalePath = FileDialogUtils.CreateLocaleFile();
        }
    }

    internal class ConfigModeEditViewModel : ConfigModeViewModel
    {
        public DelegateCommand EditCommand { get; set; }
    }
}
