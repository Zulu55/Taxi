using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Taxi.Common.Helpers;
using Taxi.Common.Models;
using Taxi.Common.Services;
using Taxi.Prism.Helpers;
using Taxi.Prism.Views;
using Xamarin.Essentials;

namespace Taxi.Prism.ViewModels
{
    public class TaxiMasterDetailPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private static TaxiMasterDetailPageViewModel _instance;
        private UserResponse _user;
        private DelegateCommand _modifyUserCommand;

        public TaxiMasterDetailPageViewModel(INavigationService navigationService, IApiService apiService)
            : base(navigationService)
        {
            _instance = this;
            _apiService = apiService;
            _navigationService = navigationService;
            LoadUser();
            LoadMenus();
        }

        public DelegateCommand ModifyUserCommand => _modifyUserCommand ?? (_modifyUserCommand = new DelegateCommand(ModifyUserAsync));

        public UserResponse User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        public ObservableCollection<MenuItemViewModel> Menus { get; set; }

        public static TaxiMasterDetailPageViewModel GetInstance()
        {
            return _instance;
        }

        public async void ReloadUser()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                return;
            }

            UserResponse user = JsonConvert.DeserializeObject<UserResponse>(Settings.User);
            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            EmailRequest emailRequest = new EmailRequest
            {
                CultureInfo = Languages.Culture,
                Email = user.Email
            };

            string url = App.Current.Resources["UrlAPI"].ToString();
            Response response = await _apiService.GetUserByEmail(url, "api", "/Account/GetUserByEmail", "bearer", token.Token, emailRequest);
            UserResponse userResponse = (UserResponse)response.Result;
            Settings.User = JsonConvert.SerializeObject(userResponse);
            LoadUser();
        }

        private async void ModifyUserAsync()
        {
            await _navigationService.NavigateAsync($"/TaxiMasterDetailPage/NavigationPage/{nameof(ModifyUserPage)}");
        }

        private void LoadUser()
        {
            if (Settings.IsLogin)
            {
                User = JsonConvert.DeserializeObject<UserResponse>(Settings.User);
            }
        }

        private void LoadMenus()
        {
            List<Menu> menus = new List<Menu>
            {
                new Menu
                {
                    Icon = "ic_airport_shuttle",
                    PageName = "HomePage",
                    Title = Languages.NewTrip
                },
                new Menu
                {
                    Icon = "ic_local_taxi",
                    PageName = "TaxiHistoryPage",
                    Title = Languages.SeeTaxiHistory
                },
                new Menu
                {
                    Icon = "ic_location_on",
                    PageName = "MyTripsPage",
                    Title = Languages.MyTrips,
                    IsLoginRequired = true
                },
                new Menu
                {
                    Icon = "ic_people",
                    PageName = "GroupPage",
                    Title = Languages.AdminMyUserGroup,
                    IsLoginRequired = true
                },
                new Menu
                {
                    Icon = "ic_account_circle",
                    PageName = "ModifyUserPage",
                    Title = Languages.ModifyUser,
                    IsLoginRequired = true
                },
                new Menu
                {
                    Icon = "ic_report",
                    PageName = "ReportPage",
                    Title = Languages.ReportAnIncident
                },
                new Menu
                {
                    Icon = "ic_exit_to_app",
                    PageName = "LoginPage",
                    Title = Settings.IsLogin ? Languages.Logout : Languages.LogIn
                }
            };

            Menus = new ObservableCollection<MenuItemViewModel>(
                menus.Select(m => new MenuItemViewModel(_navigationService)
                {
                    Icon = m.Icon,
                    PageName = m.PageName,
                    Title = m.Title,
                    IsLoginRequired = m.IsLoginRequired
                }).ToList());
        }
    }
}
