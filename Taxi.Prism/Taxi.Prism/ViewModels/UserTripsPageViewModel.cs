using Prism.Navigation;
using Taxi.Common.Models;
using Taxi.Prism.Helpers;

namespace Taxi.Prism.ViewModels
{
    public class UserTripsPageViewModel : ViewModelBase
    {
        private UserGroupDetailResponse _user;

        public UserTripsPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = Languages.TripsOf;
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            _user = parameters.GetValue<UserGroupDetailResponse>("user");
            Title = $"{Languages.TripsOf}: {_user.User.FullName}";
        }
    }
}
