using System;
using WPFLocales.Tool.ViewModels.Common;

namespace WPFLocales.Tool.ViewModels.Config
{
    internal class ConfigModeViewModel : ViewModelBase
    {
        public event Action Completed = () => { };

        protected void RaiseCompleted()
        {
            Completed();
        }
    }

    internal class ConfigModeEditViewModel : ConfigModeViewModel
    {
        public DelegateCommand EditCommand { get; set; }
    }
}
