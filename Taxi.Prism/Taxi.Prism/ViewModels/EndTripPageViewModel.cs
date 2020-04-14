using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Taxi.Common.Helpers;
using Taxi.Common.Models;
using Taxi.Common.Services;
using Taxi.Prism.Helpers;
using Xamarin.Forms.Maps;

namespace Taxi.Prism.ViewModels
{
    public class EndTripPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private readonly IGeolocatorService _geolocatorService;
        private int _tripId;
        private bool _isRunning;
        private bool _isEnabled;
        private float _qualification;
        private Comment _comment;
        private ObservableCollection<Comment> _comments;
        private string _remark;
        private double _distance;
        private string _time;
        private string _value;
        private DelegateCommand _endTripCommand;

        public EndTripPageViewModel(INavigationService navigationService, IApiService apiService, IGeolocatorService geolocatorService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            _geolocatorService = geolocatorService;
            Title = Languages.EndTrip;
            IsEnabled = true;
            Comments = new ObservableCollection<Comment>(CombosHelper.GetComments());
        }

        public DelegateCommand EndTripCommand => _endTripCommand ?? (_endTripCommand = new DelegateCommand(EndTripAsync));

        public string Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        public double Distance
        {
            get => _distance;
            set => SetProperty(ref _distance, value);
        }

        public string Time
        {
            get => _time;
            set => SetProperty(ref _time, value);
        }

        public string Remark
        {
            get => _remark;
            set => SetProperty(ref _remark, value);
        }

        public Comment Comment
        {
            get => _comment;
            set
            {
                Comment comment = value;
                Remark += string.IsNullOrEmpty(Remark) ? $"{comment.Name}" : $", {comment.Name}";
                SetProperty(ref _comment, value);
            }
        }

        public ObservableCollection<Comment> Comments
        {
            get => _comments;
            set => SetProperty(ref _comments, value);
        }

        public float Qualification
        {
            get => _qualification;
            set => SetProperty(ref _qualification, value);
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

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            _tripId = parameters.GetValue<int>("tripId");
            LoadTripAsync(_tripId);
        }

        private async void LoadTripAsync(int id)
        {
            IsRunning = true;
            IsEnabled = false;

            string url = App.Current.Resources["UrlAPI"].ToString();
            if (!_apiService.CheckConnection())
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.ConnectionError,
                    Languages.Accept);
                return;
            }

            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            Response response = await _apiService.GetTripAsync(url, "api", "/Trips", id, "bearer", token.Token);

            IsRunning = false;
            IsEnabled = true;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    response.Message,
                    Languages.Accept);
                return;
            }

            TripResponse trip = (TripResponse)response.Result;
            TripSummary tripSummary = GeoHelper.GetTripSummary(trip);
            Distance = tripSummary.Distance;
            Time = $"{tripSummary.Time.ToString().Substring(0, 8)}";
            Value = $"{tripSummary.Value:C0}";
        }

        private async void EndTripAsync()
        {
            if (Qualification == 0)
            {
                await App.Current.MainPage.DisplayAlert(Languages.Error, Languages.QualificationError, Languages.Accept);
                return;
            }

            IsRunning = true;
            IsEnabled = false;

            string url = App.Current.Resources["UrlAPI"].ToString();
            if (!_apiService.CheckConnection())
            {
                IsRunning = false;
                IsEnabled = true;
                await App.Current.MainPage.DisplayAlert(Languages.Error, Languages.ConnectionError, Languages.Accept);
                return;
            }

            await _geolocatorService.GetLocationAsync();
            Position position = new Position();
            string address = string.Empty;

            if (_geolocatorService.Latitude != 0 && _geolocatorService.Longitude != 0)
            {
                position = new Position(_geolocatorService.Latitude, _geolocatorService.Longitude);
                Geocoder geoCoder = new Geocoder();
                IEnumerable<string> sources = await geoCoder.GetAddressesForPositionAsync(position);
                List<string> addresses = new List<string>(sources);
                if (addresses.Count > 1)
                {
                    address = addresses[0];
                }
            }

            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);

            CompleteTripRequest completeTripRequest = new CompleteTripRequest
            {
                Qualification = Qualification,
                Remarks = Remark,
                Target = address,
                TargetLatitude = position.Latitude,
                TargetLongitude = position.Longitude,
                TripId = _tripId
            };

            _apiService.CompleteTripAsync(url, "/api", "/Trips/CompleteTrip", completeTripRequest, "bearer", token.Token);
            await _navigationService.GoBackToRootAsync();
        }
    }
}
