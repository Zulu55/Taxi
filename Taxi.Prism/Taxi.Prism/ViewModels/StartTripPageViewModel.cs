using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private Geocoder _geoCoder;
        private TripDetailsRequest _tripDetailsRequest;
        private DelegateCommand _getAddressCommand;
        private DelegateCommand _startTripCommand;

        public StartTripPageViewModel(INavigationService navigationService, IGeolocatorService geolocatorService, IApiService apiService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _geolocatorService = geolocatorService;
            _apiService = apiService;
            _tripDetailsRequest = new TripDetailsRequest { TripDetails = new List<TripDetailRequest>() };
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

        public string ButtonLabel
        {
            get => _source;
            set => SetProperty(ref _source, value);
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

        public string Source
        {
            get => _buttonLabel;
            set => SetProperty(ref _buttonLabel, value);
        }

        private async void LoadSourceAsync()
        {
            IsEnabled = false;
            await _geolocatorService.GetLocationAsync();

            if (_geolocatorService.Latitude == 0 && _geolocatorService.Longitude == 0)
            {
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert(Languages.Error, Languages.GeolocationError, Languages.Accept);
                await _navigationService.GoBackAsync();
                return;
            }

            _position = new Position(_geolocatorService.Latitude, _geolocatorService.Longitude);
            Geocoder geoCoder = new Geocoder();
            IEnumerable<string> sources = await geoCoder.GetAddressesForPositionAsync(_position);
            List<string> addresses = new List<string>(sources);

            if (addresses.Count > 1)
            {
                Source = addresses[0];
            }

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

            string url = App.Current.Resources["UrlAPI"].ToString();
            bool connection = await _apiService.CheckConnectionAsync(url);
            if (!connection)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert(Languages.Error, Languages.ConnectionError, Languages.Accept);
                return;
            }

            UserResponse user = JsonConvert.DeserializeObject<UserResponse>(Settings.User);
            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            TripRequest tripRequest = new TripRequest
            {
                Address = Source,
                Latitude = _geolocatorService.Latitude,
                Longitude = _geolocatorService.Longitude,
                Plaque = Plaque,
                UserId = new Guid(user.Id)
            };

            Response response = await _apiService.NewTripAsync(url, "/api", "/Trips", tripRequest, "bearer", token.Token);

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
            StartTripPage.GetInstance().AddPin(_position, Source, Languages.StartTrip, PinType.Place);
            IsRunning = false;
            IsEnabled = true;

            _timer = new Timer
            {
                Interval = 1000
            };

            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }

        private async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            await _geolocatorService.GetLocationAsync();
            if (_geolocatorService.Latitude == 0 && _geolocatorService.Longitude == 0)
            {
                return;
            }

            Position previousPosition = new Position(_position.Latitude, _position.Longitude);
            _position = new Position(_geolocatorService.Latitude, _geolocatorService.Longitude);
            double distance = GeoHelper.GetDistance(previousPosition, _position, UnitOfLength.Kilometers);

            if (distance < 0.003 || double.IsNaN(distance))
            {
                return;
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                StartTripPage.GetInstance().DrawLine(previousPosition, _position);
            });

            _tripDetailsRequest.TripDetails.Add(new TripDetailRequest
            {
                Latitude = _position.Latitude,
                Longitude = _position.Longitude,
                TripId = _tripResponse.Id
            });

            if (_tripDetailsRequest.TripDetails.Count > 9)
            {
                SendTripDetailsAsync();
            }
        }

        private async Task SendTripDetailsAsync()
        {
            TripDetailsRequest tripDetailsRequestCloned = CloneTripDetailsRequest(_tripDetailsRequest);
            _tripDetailsRequest.TripDetails.Clear();
            await _apiService.AddTripDetailsAsync(_url, "/api", "/Trips/AddTripDetails", tripDetailsRequestCloned, "bearer", _token.Token);
        }

        private TripDetailsRequest CloneTripDetailsRequest(TripDetailsRequest tripDetailsRequest)
        {
            TripDetailsRequest tripDetailsRequestCloned = new TripDetailsRequest
            {
                TripDetails = tripDetailsRequest.TripDetails.Select(d => new TripDetailRequest
                {
                    Address = d.Address,
                    Latitude = d.Latitude,
                    Longitude = d.Longitude,
                    TripId = d.TripId
                }).ToList()
            };

            return tripDetailsRequestCloned;
        }

        private async Task<bool> ValidateDataAsync()
        {
            if (string.IsNullOrEmpty(Plaque))
            {
                await App.Current.MainPage.DisplayAlert(Languages.Error, Languages.PlaqueError1, Languages.Accept);
                return false;
            }

            Regex regex = new Regex(@"^([A-Za-z]{3}\d{3})$");
            if (!regex.IsMatch(Plaque))
            {
                await App.Current.MainPage.DisplayAlert(Languages.Error, Languages.PlaqueError2, Languages.Accept);
                return false;
            }

            return true;
        }
    }
}
