using System.Collections.ObjectModel;
using WPFLocales.Tool.ViewModels.Translate;

namespace WPFLocales.Tool.SampleData
{
    class DesignTranslateLocaleViewModel : TranslateLocaleViewModel
    {
        public DesignTranslateLocaleViewModel()
        {
            Groups = new ObservableCollection<TranslateGroupViewModel>
            {
                new DesignTranslateGroupViewModel
                {
                    Key = "Group #1",
                    Items = new ObservableCollection<TranslateItemViewModel>
                    {
                        new TranslateItemViewModel { Key = "Item #1", Comment = "Comment #1", DefaultValue = "Default value #1", NewValue = "Translated value #1" },
                        new TranslateItemViewModel { Key = "Item #2", Comment = "Comment #2", DefaultValue = "Default value #2", NewValue = "Translated value #2" },
                        new TranslateItemViewModel { Key = "Item #3", Comment = "Comment #3", DefaultValue = "Default value #3", NewValue = "Translated value #3" },
                        new TranslateItemViewModel { Key = "Item #4", Comment = "Comment #4", DefaultValue = "Default value #4", NewValue = "Translated value #4" },
                        new TranslateItemViewModel { Key = "Item #5", Comment = "Comment #5", DefaultValue = "Default value #5", NewValue = "Translated value #5" },
                        new TranslateItemViewModel { Key = "Item #6", Comment = "Comment #6", DefaultValue = "Default value #6", NewValue = "Translated value #6" }
                    }
                },
                new DesignTranslateGroupViewModel
                {
                    Key = "Group #2",
                    Items = new ObservableCollection<TranslateItemViewModel>
                    {
                        new TranslateItemViewModel { Key = "Item #1", Comment = "Comment #1", DefaultValue = "Default value #1", NewValue = "Translated value #1" },
                        new TranslateItemViewModel { Key = "Item #2", Comment = "Comment #2", DefaultValue = "Default value #2", NewValue = "Translated value #2" },
                        new TranslateItemViewModel { Key = "Item #3", Comment = "Comment #3", DefaultValue = "Default value #3", NewValue = "Translated value #3" },
                        new TranslateItemViewModel { Key = "Item #4", Comment = "Comment #4", DefaultValue = "Default value #4", NewValue = "Translated value #4" },
                        new TranslateItemViewModel { Key = "Item #5", Comment = "Comment #5", DefaultValue = "Default value #5", NewValue = "Translated value #5" },
                        new TranslateItemViewModel { Key = "Item #6", Comment = "Comment #6", DefaultValue = "Default value #6", NewValue = "Translated value #6" }
                    }
                }
            };
        }
    }

    class DesignTranslateGroupViewModel : TranslateGroupViewModel
    {
        
    }
}
