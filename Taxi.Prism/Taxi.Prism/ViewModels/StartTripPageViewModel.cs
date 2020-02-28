using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using Taxi.Common.Helpers;
using Taxi.Common.Models;
using Taxi.Common.Services;
using Taxi.Prism.Helpers;
using Taxi.Prism.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Taxi.Prism.ViewModels
{
    public class StartTripPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IGeolocatorService _geolocatorService;
        private readonly IApiService _apiService;
        private string _source;
        private string _buttonLabel;
        private bool _isSecondButtonVisible;
        private bool _isRunning;
        private bool _isEnabled;
        private Position _position;
        private TripResponse _tripResponse;
        private UserResponse _user;
        private TokenResponse _token;
        private string _url;
        private Timer _timer;
        private DelegateCommand _getAddressCommand;
        private DelegateCommand _startTripCommand;

        public StartTripPageViewModel(INavigationService navigationService, IGeolocatorService geolocatorService, IApiService apiService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _geolocatorService = geolocatorService;
            _apiService = apiService;
            Title = Languages.StartTrip;
            ButtonLabel = Languages.StartTrip;
            IsEnabled = true;
            LoadSourceAsync();
        }

        public DelegateCommand GetAddressCommand => _getAddressCommand ?? (_getAddressCommand = new DelegateCommand(LoadSourceAsync));

        public DelegateCommand StartTripCommand => _startTripCommand ?? (_startTripCommand = new DelegateCommand(StartTripAsync));

        public string Plaque { get; set; }

        public bool IsSecondButtonVisible
        {
            get => _isSecondButtonVisible;
            set => SetProperty(ref _isSecondButtonVisible, value);
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetProperty(ref _isEnabled, value);
        }

        public string ButtonLabel
        {
            get => _source;
            set => SetProperty(ref _source, value);
        }

        public string Source
        {
            get => _buttonLabel;
            set => SetProperty(ref _buttonLabel, value);
        }

        private async void LoadSourceAsync()
        {
            IsRunning = true;
            IsEnabled = false;
            await _geolocatorService.GetLocationAsync();

            if (_geolocatorService.Latitude == 0 && _geolocatorService.Longitude == 0)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert(Languages.Error, Languages.GeolocationError, Languages.Accept);
                await _navigationService.GoBackAsync();
                return;
            }

            _position = new Position(_geolocatorService.Latitude, _geolocatorService.Longitude);
            Geocoder geoCoder = new Geocoder();
            IEnumerable<string> sources = await geoCoder.GetAddressesForPositionAsync(_position);
            List<string> addresses = new List<string>(sources);

            if (addresses.Count == 1)
            {
                Source = addresses[0];
            }

            if (addresses.Count > 1)
            {
                string address = await Application.Current.MainPage.DisplayActionSheet(
                    Languages.ConfirmAddress,
                    Languages.Cancel,
                    null,
                    addresses.ToArray());
                if (address != Languages.Cancel)
                {
                    Source = address;
                }
            }

            IsRunning = false;
            IsEnabled = true;
        }

        private async void StartTripAsync()
        {
            bool isValid = await ValidateDataAsync();
            if (!isValid)
            {
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            _url = App.Current.Resources["UrlAPI"].ToString();
            bool connection = await _apiService.CheckConnectionAsync(_url);
            if (!connection)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.ConnectionError,
                    Languages.Accept);
                return;
            }

            _user = JsonConvert.DeserializeObject<UserResponse>(Settings.User);
            _token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            TripRequest tripRequest = new TripRequest
            {
                Address = Source,
                Latitude = _geolocatorService.Latitude,
                Longitude = _geolocatorService.Longitude,
                Plaque = Plaque,
                UserId = new Guid(_user.Id)
            };

            Response response = await _apiService.NewTripAsync(_url, "/api", "/Trips", tripRequest, "bearer", _token.Token);

            if (!response.IsSuccess)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert(Languages.Error, response.Message, Languages.Accept);
                return;
            }

            _tripResponse = (TripResponse)response.Result;
            IsSecondButtonVisible = true;
            ButtonLabel = Languages.EndTrip;
            StartTripPage.GetInstance().AddPin(_position, Source, Languages.StartTrip);
            IsRunning = false;
            IsEnabled = true;

            _timer = new Timer
            {
                Interval = 5000
            };

            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            bool connection = await _apiService.CheckConnectionAsync(_url);
            if (!connection)
            {
                return;
            }

            await _geolocatorService.GetLocationAsync();
            if (_geolocatorService.Latitude == 0 && _geolocatorService.Longitude == 0)
            {
                return;
            }

            Position previousPosition = new Position(_position.Latitude, _position.Longitude);
            _position = new Position(_geolocatorService.Latitude, _geolocatorService.Longitude);
            double distance = GeoHelper.GetDistance(previousPosition, _position, UnitOfLength.Kilometers);

            if (distance < 0.01 || double.IsNaN(distance))
            {
                return;
            }

            Geocoder geoCoder = new Geocoder();
            IEnumerable<string> sources = await geoCoder.GetAddressesForPositionAsync(_position);
            List<string> addresses = new List<string>(sources);

            TripDetailRequest tripDetailRequest = new TripDetailRequest
            {
                Address = addresses.Count > 0 ? addresses[0] : null,
                Latitude = _position.Latitude,
                Longitude = _position.Longitude,
                TripId = _tripResponse.Id
            };

            Response response = await _apiService.AddTripDetailAsync(_url, "/api", "/Trips/AddTripDetail", tripDetailRequest, "bearer", _token.Token);

            if (!response.IsSuccess)
            {
                return;
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                StartTripPage.GetInstance().DrawLine(previousPosition, _position);
            });
        }

        private async Task<bool> ValidateDataAsync()
        {
            if (string.IsNullOrEmpty(Plaque))
            {
                await App.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.PlaqueError1,
                    Languages.Accept);
                return false;
            }

            Regex regex = new Regex(@"^([A-Za-z]{3}\d{3})$");
            if (!regex.IsMatch(Plaque))
            {
                await App.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.PlaqueError2,
                    Languages.Accept);
                return false;
            }

            return true;
        }
    }
}
