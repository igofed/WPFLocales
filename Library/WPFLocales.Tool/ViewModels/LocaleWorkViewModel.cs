using WPFLocales.Tool.ViewModels.Common;

namespace WPFLocales.Tool.ViewModels
{
    internal abstract class LocaleWorkViewModel : ViewModelBase
    {
        public DelegateCommand SaveCommand { get; set; }


        protected LocaleWorkViewModel()
        {
            InitCommands();
        }


        private void InitCommands()
        {
            SaveCommand = new DelegateCommand(SaveCommandExecute, CanSaveCommandExecute);
        }


        protected abstract void SaveCommandExecute();

        protected abstract bool CanSaveCommandExecute();
    }
}
