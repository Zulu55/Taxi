using Prism.Navigation;

namespace Taxi.Prism.ViewModels
{
    public class ReportPageViewModel : ViewModelBase
    {
        public ReportPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "Report an incident";
        }
    }
}
