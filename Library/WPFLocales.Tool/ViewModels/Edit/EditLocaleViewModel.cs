using System.Collections.ObjectModel;

namespace WPFLocales.Tool.ViewModels.Edit
{
    internal class EditLocaleViewModel : LocaleWorkViewModel
    {
        public string Key { get; set; }
        public ObservableCollection<EditGroupViewModel> Groups { get; set; }
    }

    internal class EditGroupViewModel
    {
        public string Key { get; set; }
        public ObservableCollection<EditItemViewModel> Items { get; set; }
    }

    internal class EditItemViewModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string Comment { get; set; }
    }
}
