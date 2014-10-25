using System;
using WpfLocales.Model.Xml;
using WPFLocales.Tool.ViewModels.Common;

namespace WPFLocales.Tool.ViewModels.Translate
{
    internal class TranslateItemViewModel : ViewModelBase
    {
        public event Action IsTranslateChanged = () => { }; 

        public string Key { get; set; }
        public string Comment { get; set; }
        public string DefaultValue { get; set; }
        public string NewValue
        {
            get { return _newValue; }
            set
            {
                if (Set(ref _newValue, value, () => NewValue))
                    IsTranslated = !string.Equals(DefaultValue, NewValue);
            }
        }
        public bool IsTranslated
        {
            get { return _isTranslated; }
            set { if (Set(ref _isTranslated, value, () => IsTranslated)) IsTranslateChanged(); }
        }


        private string _newValue;
        private bool _isTranslated;


        public XmlLocaleItem ToItem()
        {
            return new XmlLocaleItem
            {
                Key = Key,
                Comment = Comment,
                Value = NewValue
            };
        }
    }
}