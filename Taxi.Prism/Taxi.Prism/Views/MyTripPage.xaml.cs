using Taxi.Common.Models;
using Taxi.Prism.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Taxi.Prism.Views
{
    public partial class MyTripPage : ContentPage
    {
        private static MyTripPage _instance;

        public MyTripPage()
        {
            InitializeComponent();
            _instance = this;
        }

        public static MyTripPage GetInstance()
        {
            return _instance;
        }

        public void DrawMap(TripResponse trip)
        {
            if (trip.SourceLatitude != 0 && trip.SourceLongitude != 0)
            {
                Position position = new Position(trip.SourceLatitude, trip.SourceLongitude);
                AddPin(position, trip.Source, Languages.StartTrip, PinType.Place);
                MoveMap(position);
            }

            if (trip.TargetLatitude != 0 && trip.TargetLongitude != 0)
            {
                Position position = new Position(trip.TargetLatitude, trip.TargetLongitude);
                AddPin(position, trip.Target, Languages.EndTrip, PinType.Place);
                MoveMap(position);
            }

            for (int i = 0; i < trip.TripDetails.Count - 1; i++)
            {
                Position a = new Position(trip.TripDetails[i].Latitude, trip.TripDetails[i].Longitude);
                Position b = new Position(trip.TripDetails[i + 1].Latitude, trip.TripDetails[i + 1].Longitude);
                DrawLine(a, b);
            }
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
        }


        private void AddPin(Position position, string address, string label, PinType pinType)
        {
            MyMap.Pins.Add(new Pin
            {
                Address = address,
                Label = label,
                Position = position,
                Type = pinType
            });
        }

        private void MoveMap(Position position)
        {
            MyMap.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(.5)));
        }
    }
}
