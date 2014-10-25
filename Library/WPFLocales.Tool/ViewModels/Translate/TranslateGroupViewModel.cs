using System;
using System.Collections.ObjectModel;
using System.Linq;
using WpfLocales.Model.Xml;
using WPFLocales.Tool.ViewModels.Common;

namespace WPFLocales.Tool.ViewModels.Translate
{
    internal class TranslateGroupViewModel : ViewModelBase
    {
        public event Action Changed = () => { }; 

        public string Key { get; set; }
        public string Comment { get; set; }
        public ObservableCollection<TranslateItemViewModel> Items { get; set; }
        public int TranslatedCount
        {
            get { return _translatedCount; }
            set
            {
                if (Set(ref _translatedCount, value, () => TranslatedCount))
                    IsTranslated = TranslatedCount == Items.Count;
            }
        }
        public bool IsTranslated
        {
            get { return _isTranslated; }
            set { Set(ref _isTranslated, value, () => IsTranslated); }
        }


        private int _translatedCount;
        private bool _isTranslated;


        protected TranslateGroupViewModel()
        {

        }

        public TranslateGroupViewModel(XmlLocaleGroup defaultGroup, XmlLocaleGroup newGroup)
        {
            Key = defaultGroup.Key;
            Comment = defaultGroup.Comment;
            Items = new ObservableCollection<TranslateItemViewModel>();

            if (defaultGroup.Items == null)
                return;

            foreach (var defaultItem in defaultGroup.Items)
            {
                XmlLocaleItem newItem = null;
                if (newGroup != null && newGroup.Items != null)
                    newItem = newGroup.Items.FirstOrDefault(i => i.Key == defaultItem.Key);

                var newItemViewModel = new TranslateItemViewModel
                {
                    Key = defaultItem.Key,
                    Comment = defaultItem.Comment,
                    DefaultValue = defaultItem.Value,
                    NewValue = newItem != null ? newItem.Value : defaultItem.Value
                };
                newItemViewModel.IsTranslateChanged += OnIsTranslateChanged;
                Items.Add(newItemViewModel);
            }

            TranslatedCount = Items.Count(i => i.IsTranslated);
        }

        private void OnIsTranslateChanged()
        {
            TranslatedCount = Items.Count(i => i.IsTranslated);

            Changed();
        }

        public XmlLocaleGroup ToGroup()
        {
            return new XmlLocaleGroup
            {
                Key = Key,
                Comment = Comment,
                Items = Items.Select(i=>i.ToItem()).ToList()
            };
        }
    }
}