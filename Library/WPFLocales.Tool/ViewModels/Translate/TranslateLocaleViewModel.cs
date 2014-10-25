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


        private readonly LocaleContainer _newLocale;
        private bool _isAnyChanges;

        protected TranslateLocaleViewModel()
        {
            
        }

        public TranslateLocaleViewModel(LocaleContainer defaultLocale, LocaleContainer newLocale)
        {
            _newLocale = newLocale;

            DefaultLocaleKey = defaultLocale.Locale.Key;
            NewLocaleKey = newLocale.Locale.Key;

            Groups = new ObservableCollection<TranslateGroupViewModel>();
            if (defaultLocale.Locale.Groups != null)
            {
                foreach (var defaultGroup in defaultLocale.Locale.Groups)
                {
                    XmlLocaleGroup newGroup = null;
                    if (newLocale.Locale.Groups != null)
                        newGroup = newLocale.Locale.Groups.FirstOrDefault(lg => lg.Key == defaultGroup.Key);

                    var groupViewModel = new TranslateGroupViewModel(defaultGroup, newGroup);
                    groupViewModel.Changed += OnGroupChanged;
                    Groups.Add(groupViewModel);
                }
            }
        }

        private void OnGroupChanged()
        {
            _isAnyChanges = true;
            SaveCommand.RaiseCanExecuteChanged();
        }


        protected override void SaveCommandExecute()
        {
            var locale = new XmlLocale
            {
                Key = _newLocale.Locale.Key,
                Title = _newLocale.Locale.Title,
                Groups = Groups.Select(g => g.ToGroup()).ToList()
            };
            var localeContainer = new LocaleContainer { Locale = locale, Path = _newLocale.Path };
            LocaleContainer.WriteToFile(localeContainer);

            _isAnyChanges = false;
            SaveCommand.RaiseCanExecuteChanged();
        }

        protected override bool CanSaveCommandExecute()
        {
            return _isAnyChanges;
        }
    }
}
