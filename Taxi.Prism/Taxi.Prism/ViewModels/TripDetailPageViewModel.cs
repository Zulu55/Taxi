using Prism.Navigation;
using Taxi.Common.Models;

namespace Taxi.Prism.ViewModels
{
    public class TripDetailPageViewModel : ViewModelBase
    {
        private TripResponse _trip;

        public TripDetailPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "Trip Details";
        }

        public TripResponse Trip
        {
            get => _trip;
            set => SetProperty(ref _trip, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            if (parameters.ContainsKey("trip"))
            {
                Trip = parameters.GetValue<TripResponse>("trip");
            }
        }
    }
}