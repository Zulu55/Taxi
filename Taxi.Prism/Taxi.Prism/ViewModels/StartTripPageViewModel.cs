using Prism.Commands;
using Prism.Navigation;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Taxi.Common.Services;
using Taxi.Prism.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Taxi.Prism.ViewModels
{
    public class StartTripPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IGeolocatorService _geolocatorService;
        private string _source;
        private string _buttonLabel;
        private bool _isSecondButtonVisible;
        private DelegateCommand _getAddressCommand;
        private DelegateCommand _startTripCommand;

        public StartTripPageViewModel(INavigationService navigationService, IGeolocatorService geolocatorService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _geolocatorService = geolocatorService;
            Title = Languages.StartTrip;
            ButtonLabel = Languages.StartTrip;
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

        public string Source
        {
            get => _buttonLabel;
            set => SetProperty(ref _buttonLabel, value);
        }

        private async void LoadSourceAsync()
        {
            await _geolocatorService.GetLocationAsync();
            if (_geolocatorService.Latitude != 0 && _geolocatorService.Longitude != 0)
            {
                Position position = new Position(_geolocatorService.Latitude, _geolocatorService.Longitude);
                Geocoder geoCoder = new Geocoder();
                IEnumerable<string> sources = await geoCoder.GetAddressesForPositionAsync(position);
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
            }
        }

        private async void StartTripAsync()
        {
            bool isValid = await ValidateDataAsync();
            if (!isValid)
            {
                return;
            }


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
