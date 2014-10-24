using WPFLocales.Tool.Models;
using WPFLocales.Tool.ViewModels.Common;
using WPFLocales.Tool.ViewModels.Config;
using WPFLocales.Tool.ViewModels.Edit;
using WPFLocales.Tool.ViewModels.Translate;

namespace WPFLocales.Tool.ViewModels
{
    internal class MainViewModel : ViewModelBase
    {
        public WorkMode? WorkMode
        {
            get { return _workMode; }
            set { Set(ref _workMode, value, () => WorkMode); }
        }
        public LocaleWorkViewModel Work
        {
            get { return _work; }
            set { Set(ref _work, value, () => Work); }
        }
        public ConfigViewModel Config { get; set; }


        private WorkMode? _workMode;
        private LocaleWorkViewModel _work;


        public MainViewModel(LocaleContainer defaultLocale = null, LocaleContainer newLocale = null)
        {
            Config = new ConfigViewModel();
            Config.EditConfigured += OnEditConfigured;
            Config.TranslateConfigured += OnTranslateConfigured;

            InitCommands();
        }

        private void OnTranslateConfigured(LocaleContainer defaultLocale, LocaleContainer newLocale)
        {
           Work = new TranslateLocaleViewModel();
           WorkMode = ViewModels.WorkMode.Work;
        }

        private void OnEditConfigured(LocaleContainer defaultLocale)
        {
            Work = new EditLocaleViewModel();
            WorkMode = ViewModels.WorkMode.Work;
        }


        private void InitCommands()
        {

        }
    }

    internal enum WorkMode
    {
        Config,
        Work
    }
}
