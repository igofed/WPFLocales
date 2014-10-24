using WPFLocales.Tool.ViewModels.Common;

namespace WPFLocales.Tool.ViewModels
{
    internal class ConfigViewModel : ViewModelBase
    {
        public bool IsEdit
        {
            get { return _isEdit; }
            set 
            { 
                _isEdit = value;
                if (value) ConfigMode = _editMode;
            }
        }
        public bool IsTranslate
        {
            get { return _isTranslate; }
            set
            {
                _isTranslate = value;
                if (value) ConfigMode = _translateMode;
            }
        }
        public ConfigModeViewModel ConfigMode
        {
            get { return _configMode; }
            set { Set(ref _configMode, value, () => ConfigMode); }
        }


        private bool _isTranslate;
        private bool _isEdit;
        private ConfigModeViewModel _configMode;

        private readonly ConfigModeEditViewModel _editMode;
        private readonly ConfigModeTranslateViewModel _translateMode;


        public ConfigViewModel()
        {
            _editMode = new ConfigModeEditViewModel();
            _translateMode = new ConfigModeTranslateViewModel();

            IsTranslate = true;
        }
    }
}
