using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Taxi.Common.Helpers;
using Taxi.Common.Models;
using Taxi.Common.Services;
using Taxi.Prism.Helpers;
using Taxi.Prism.Views;
using Xamarin.Essentials;
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
        private readonly Geocoder _geoCoder;
        private readonly TripDetailsRequest _tripDetailsRequest;
        private DelegateCommand _getAddressCommand;
        private DelegateCommand _startTripCommand;
        private DelegateCommand _cancelTripCommand;

        public StartTripPageViewModel(INavigationService navigationService, IGeolocatorService geolocatorService, IApiService apiService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _geolocatorService = geolocatorService;
            _apiService = apiService;
            _tripDetailsRequest = new TripDetailsRequest { TripDetails = new List<TripDetailRequest>() };
            _geoCoder = new Geocoder();
            Title = Languages.StartTrip;
            ButtonLabel = Languages.StartTrip;
            IsEnabled = true;
            LoadSourceAsync();
        }

        public DelegateCommand CancelTripCommand => _cancelTripCommand ?? (_cancelTripCommand = new DelegateCommand(CancelTripAsync));

        public DelegateCommand GetAddressCommand => _getAddressCommand ?? (_getAddressCommand = new DelegateCommand(LoadSourceAsync));

        public DelegateCommand StartTripCommand => _startTripCommand ?? (_startTripCommand = new DelegateCommand(StartTripAsync));

        public string PlaqueLetters { get; set; }

        public int? PlaqueNumbers { get; set; }

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

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (IsSecondButtonVisible && _timer != null)
            {
                _timer.Start();
            }
        }

        private async void CancelTripAsync()
        {
            bool answer = await App.Current.MainPage.DisplayAlert(Languages.Confirmation, Languages.CancelTripConfirm, Languages.Yes, Languages.No);
            if (!answer)
            {
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            _timer.Stop();
            bool connection = await _apiService.CheckConnectionAsync(_url);
            if (!connection)
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert(Languages.Error, Languages.ConnectionError, Languages.Accept);
                return;
            }

            _apiService.DeleteAsync(_url, "/api", "/Trips", _tripResponse.Id, "bearer", _token.Token);

            IsRunning = false;
            IsEnabled = true;

            await _navigationService.GoBackToRootAsync();
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

            if (addresses.Count > 0)
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

            if (IsSecondButtonVisible)
            {
                await EndTripAsync();
            }
            else
            {
                await BeginTripAsync();
            }
        }

        private async Task BeginTripAsync()
        {
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
                Plaque = $"{PlaqueLetters}{PlaqueNumbers}",
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
            StartTripPage.GetInstance().AddPin(_position, Source, Languages.StartTrip, PinType.Place);
            IsRunning = false;
            IsEnabled = true;

            _timer = new Timer
            {
                Interval = 2000
            };

            _timer.Elapsed += Timer_Elapsed;
            _timer.Start();
        }

        private async Task EndTripAsync()
        {
            _timer.Stop();

            if (_tripDetailsRequest.TripDetails.Count > 0)
            {
                await SendTripDetailsAsync();
            }

            NavigationParameters parameters = new NavigationParameters
            {
                { "tripId", _tripResponse.Id },
            };

            await _navigationService.NavigateAsync(nameof(EndTripPage), parameters);
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
            if (string.IsNullOrEmpty(PlaqueLetters) || PlaqueNumbers == 0)
            {
                await App.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.PlaqueError1,
                    Languages.Accept);
                return false;
            }

            return true;
        }
    }
}
