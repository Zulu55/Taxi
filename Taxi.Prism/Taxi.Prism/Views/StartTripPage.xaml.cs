using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System.Threading.Tasks;
using Taxi.Common.Services;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Taxi.Prism.Views
{
    public partial class StartTripPage : ContentPage
    {
        private readonly IGeolocatorService _geolocatorService;
        private static StartTripPage _instance;
        private double _distance;
        private Position _position;

        public StartTripPage(IGeolocatorService geolocatorService)
        {
            InitializeComponent();
            _geolocatorService = geolocatorService;
            _instance = this;
            _distance = .2;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            MoveMapToCurrentPositionAsync();
        }

        public static StartTripPage GetInstance()
        {
            return _instance;
        }

        public void AddPin(Position position, string address, string label, PinType pinType)
        {
            _position = position;
            MyMap.Pins.Add(new Pin
            {
                Address = address,
                Label = label,
                Position = position,
                Type = pinType
            });
        }

        public void DrawLine(Position a, Position b)
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                Polygon polygon = new Polygon
                {
                    StrokeWidth = 10,
                    StrokeColor = Color.FromHex("#8D07F6"),
                    FillColor = Color.FromHex("#8D07F6"),
                    Geopath = { a, b }
                };

                MyMap.MapElements.Add(polygon);
            }
            else
            {
                AddPin(b, string.Empty, string.Empty, PinType.SavedPin);
            }

            _position = b;
            MoveMap();
        }

        private async void MoveMapToCurrentPositionAsync()
        {
            bool isLocationPermision = await CheckLocationPermisionsAsync();

            if (isLocationPermision)
            {
                MyMap.IsShowingUser = true;

                await _geolocatorService.GetLocationAsync();
                if (_geolocatorService.Latitude != 0 && _geolocatorService.Longitude != 0)
                {
                    _position = new Position(
                        _geolocatorService.Latitude,
                        _geolocatorService.Longitude);
                    MoveMap();
                }
            }
        }

        private void MoveMap()
        {
            MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(_position, Distance.FromKilometers(_distance)));
        }

        private async Task<bool> CheckLocationPermisionsAsync()
        {
            PermissionStatus permissionLocation = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
            PermissionStatus permissionLocationAlways = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.LocationAlways);
            PermissionStatus permissionLocationWhenInUse = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.LocationWhenInUse);
            bool isLocationEnabled = permissionLocation == PermissionStatus.Granted ||
                                     permissionLocationAlways == PermissionStatus.Granted ||
                                     permissionLocationWhenInUse == PermissionStatus.Granted;
            if (isLocationEnabled)
            {
                return true;
            }

            await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);

            permissionLocation = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
            permissionLocationAlways = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.LocationAlways);
            permissionLocationWhenInUse = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.LocationWhenInUse);
            return permissionLocation == PermissionStatus.Granted ||
                   permissionLocationAlways == PermissionStatus.Granted ||
                   permissionLocationWhenInUse == PermissionStatus.Granted;
        }

        private void MySlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            _distance = e.NewValue;
            MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(_position, Distance.FromKilometers(_distance)));
        }
    }
}
