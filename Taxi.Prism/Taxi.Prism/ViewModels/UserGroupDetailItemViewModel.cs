using Prism.Commands;
using Prism.Navigation;
using Taxi.Common.Models;
using Taxi.Prism.Views;

namespace Taxi.Prism.ViewModels
{
    public class UserGroupDetailItemViewModel : UserGroupDetailResponse
    {
        private readonly INavigationService _navigationService;
        private DelegateCommand _selectUserCommand;

        public UserGroupDetailItemViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public DelegateCommand SelectUserCommand => _selectUserCommand ?? (_selectUserCommand = new DelegateCommand(SelectUserAsync));

        private async void SelectUserAsync()
        {
            NavigationParameters parameters = new NavigationParameters
            {
                { "user", this }
            };

            await _navigationService.NavigateAsync(nameof(UserTripsPage), parameters);
        }
    }
}
