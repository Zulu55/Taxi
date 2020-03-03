using Prism.Navigation;
using Taxi.Prism.Helpers;

namespace Taxi.Prism.ViewModels
{
    public class MyTripPageViewModel : ViewModelBase
    {

        public MyTripPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = Languages.MyTrip;
        }
    }
}
