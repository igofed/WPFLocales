using WPFLocales.Tool.ViewModels;
using WPFLocales.Tool.ViewModels.Config;

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
