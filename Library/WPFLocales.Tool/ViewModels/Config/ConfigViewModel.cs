using System;
using WPFLocales.Tool.Models;
using WPFLocales.Tool.ViewModels.Common;

namespace WPFLocales.Tool.ViewModels.Config
{
    internal class ConfigViewModel : ViewModelBase
    {
        public event Action<LocaleContainer> EditConfigured = e => { };
        public event Action<LocaleContainer, LocaleContainer> TranslateConfigured = (d, n) => { };
 
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
            _editMode.Completed += OnEditModeConfigCompleted;
            _translateMode = new ConfigModeTranslateViewModel();
            _translateMode.Completed += OnTranslateModeConfigCompleted;

            IsTranslate = true;
        }


        private void OnTranslateModeConfigCompleted()
        {
            TranslateConfigured(_translateMode.DefauleLocale, _translateMode.NewLocale);
        }

        private void OnEditModeConfigCompleted()
        {
            EditConfigured(null);
        }
    }
}
