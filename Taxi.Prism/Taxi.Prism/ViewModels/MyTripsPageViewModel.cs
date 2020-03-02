using Prism.Navigation;
using Taxi.Prism.Helpers;

namespace Taxi.Prism.ViewModels
{
    public class MyTripsPageViewModel : ViewModelBase
    {
        public MyTripsPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = Languages.MyTrips;
        }
    }
}
