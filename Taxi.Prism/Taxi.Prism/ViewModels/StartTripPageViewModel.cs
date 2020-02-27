using Prism.Navigation;
using Taxi.Prism.Helpers;

namespace Taxi.Prism.ViewModels
{
    public class StartTripPageViewModel : ViewModelBase
    {
        public StartTripPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = Languages.StartTrip;
        }
    }
}
