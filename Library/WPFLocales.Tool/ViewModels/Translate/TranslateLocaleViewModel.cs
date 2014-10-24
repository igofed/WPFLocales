using System.Collections.ObjectModel;

namespace WPFLocales.Tool.ViewModels.Translate
{
    internal class TranslateLocaleViewModel : LocaleWorkViewModel
    {
        public string DefaultLocaleKey { get; set; }
        public string NewLocaleKey { get; set; }
        public ObservableCollection<TranslateGroupViewModel> Groups { get; set; }
    }
}
