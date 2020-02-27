using Prism.Commands;
using Prism.Navigation;
using Taxi.Common.Helpers;
using Taxi.Prism.Views;

namespace Taxi.Prism.ViewModels
{
    public class HomePageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private DelegateCommand _startTripCommand;

        public HomePageViewModel(INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;
            Title = "Taxi Qualifier";
        }

        public DelegateCommand StartTripCommand => _startTripCommand ?? (_startTripCommand = new DelegateCommand(StartTripAsync));

        private async void StartTripAsync()
        {
            if (Settings.IsLogin)
            {
                await _navigationService.NavigateAsync(nameof(StartTripPage));
            }
            else
            {
                await _navigationService.NavigateAsync(nameof(LoginPage));
            }
        }
    }
}
