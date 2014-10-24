using WPFLocales.Tool.ViewModels;

namespace WPFLocales.Tool.SampleData
{
    internal class DesignMainViewModel : MainViewModel
    {
        public DesignMainViewModel()
        {
            WorkMode = ViewModels.WorkMode.Config;
            Config = new ConfigViewModel();
        }
    }
}
