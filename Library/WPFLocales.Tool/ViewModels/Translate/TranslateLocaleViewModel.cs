using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WPFLocales.Model;
using WpfLocales.Model.Xml;
using WPFLocales.Tool.Models;

namespace WPFLocales.Tool.ViewModels.Translate
{
    internal class TranslateLocaleViewModel : LocaleWorkViewModel
    {
        public string DefaultLocaleKey { get; set; }
        public string NewLocaleKey { get; set; }
        public ObservableCollection<TranslateGroupViewModel> Groups { get; set; }


        private readonly LocaleContainer _defaultLocale;
        private readonly LocaleContainer _newLocale;


        protected TranslateLocaleViewModel()
        {
            
        }

        public TranslateLocaleViewModel(LocaleContainer defaultLocale, LocaleContainer newLocale)
        {
            _defaultLocale = defaultLocale;
            _newLocale = newLocale;

            DefaultLocaleKey = defaultLocale.Locale.Key;
            NewLocaleKey = newLocale.Locale.Key;

            Groups = new ObservableCollection<TranslateGroupViewModel>();
            if (_defaultLocale.Locale.Groups != null)
            {
                foreach (var defaultGroup in _defaultLocale.Locale.Groups)
                {
                    var groupViewModel = new TranslateGroupViewModel { Key = defaultGroup.Key, Items = new ObservableCollection<TranslateItemViewModel>() };

                    XmlLocaleGroup newGroup = null;
                    if (newLocale.Locale.Groups != null)
                        newGroup = newLocale.Locale.Groups.FirstOrDefault(lg => lg.Key == defaultGroup.Key);

                    if (defaultGroup.Items != null)
                    {
                        foreach (var defaultItem in defaultGroup.Items)
                        {
                            XmlLocaleItem newItem = null;
                            if (newGroup != null && newGroup.Items != null)
                                newItem = newGroup.Items.FirstOrDefault(i => i.Key == defaultItem.Key);

                           groupViewModel.Items.Add(new TranslateItemViewModel
                           {
                               Key = defaultItem.Key,
                               Comment = defaultItem.Comment,
                               DefaultValue = defaultItem.Value,
                               NewValue = newItem != null ? newItem.Value : defaultItem.Value
                           });
                        }
                    }

                    Groups.Add(groupViewModel);
                }
            }
        }
    }
}
