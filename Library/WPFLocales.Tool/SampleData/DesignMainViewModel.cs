using WPFLocales.Tool.ViewModels;

namespace WPFLocales.Tool.SampleData
{
    internal class DesignMainViewModel : MainViewModel
    {
        public DesignMainViewModel()
        {
            WorkMode = ViewModels.WorkMode.Select;
        }
    }
}
