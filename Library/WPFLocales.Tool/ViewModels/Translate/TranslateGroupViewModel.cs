using System.Collections.ObjectModel;

namespace WPFLocales.Tool.ViewModels.Translate
{
    internal class TranslateGroupViewModel
    {
        public string Key { get; set; }
        public ObservableCollection<TranslateItemViewModel> Items { get; set; }
    }
}