using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using Taxi.Common.Helpers;
using Taxi.Common.Models;
using Taxi.Common.Services;
using Taxi.Prism.Helpers;
using Xamarin.Essentials;

namespace Taxi.Prism.ViewModels
{
    public class UserTripsPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private UserGroupDetailResponse _user;
        private bool _isRunning;
        private List<TripItemViewModel> _trips;
        private DelegateCommand _refreshCommand;

        public UserTripsPageViewModel(INavigationService navigationService, IApiService apiService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            Title = Languages.TripsOf;
            StartDate = DateTime.Today.AddDays(-7);
            EndDate = DateTime.Today;
        }

        public DelegateCommand RefreshCommand => _refreshCommand ?? (_refreshCommand = new DelegateCommand(LoadTripsAsync));

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public UserGroupDetailResponse User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        public List<TripItemViewModel> Trips
        {
            get => _trips;
            set => SetProperty(ref _trips, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("user"))
            {
                User = parameters.GetValue<UserGroupDetailResponse>("user");
                Title = $"{Languages.TripsOf}: {User.User.FullName}";
                LoadTripsAsync();
            }
        }

        private async void LoadTripsAsync()
        {
            IsRunning = true;

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                IsRunning = false;
                await App.Current.MainPage.DisplayAlert(Languages.Error, Languages.ConnectionError, Languages.Accept);
                return;
            }

            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            MyTripsRequest request = new MyTripsRequest
            {
                EndDate = EndDate.AddDays(1).ToUniversalTime(),
                StartDate = StartDate.ToUniversalTime(),
                UserId = User.User.Id
            };

            string url = App.Current.Resources["UrlAPI"].ToString();
            Response response = await _apiService.GetMyTrips(url, "api", "/Trips/GetMyTrips", "bearer", token.Token, request);

            IsRunning = false;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(Languages.Error, response.Message, Languages.Accept);
                return;
            }

            List<TripResponse> trips = (List<TripResponse>)response.Result;
            Trips = trips.Select(t => new TripItemViewModel(_navigationService)
            {
                EndDate = t.EndDate,
                Id = t.Id,
                Qualification = t.Qualification,
                Remarks = t.Remarks,
                Source = t.Source,
                SourceLatitude = t.SourceLatitude,
                SourceLongitude = t.SourceLongitude,
                StartDate = t.StartDate,
                Target = t.Target,
                TargetLatitude = t.TargetLatitude,
                TargetLongitude = t.TargetLongitude,
                TripDetails = t.TripDetails,
                User = t.User
            }).ToList();
        }
    }
}
