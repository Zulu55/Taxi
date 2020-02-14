using Taxi.Common.Services;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Taxi.Prism.Views
{
    public partial class HomePage : ContentPage
    {
        private readonly IGeolocatorService _geolocatorService;

        public HomePage(IGeolocatorService geolocatorService)
        {
            InitializeComponent();
            _geolocatorService = geolocatorService;
            MoveMapToCurrentPositionAsync();
        }

        private async void MoveMapToCurrentPositionAsync()
        {
            await _geolocatorService.GetLocationAsync();
            if (_geolocatorService.Latitude != 0 && _geolocatorService.Longitude != 0)
            {
                var position = new Position(
                    _geolocatorService.Latitude,
                    _geolocatorService.Longitude);
                MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(
                    position,
                    Distance.FromKilometers(.5)));
            }
        }
    }
}