using System.IO;
using WPFLocales.Tool.Models;
using WPFLocales.Tool.ViewModels.Common;

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

            InitCommands();
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
