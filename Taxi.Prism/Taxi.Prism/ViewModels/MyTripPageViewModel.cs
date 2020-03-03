using Prism.Navigation;
using Taxi.Common.Models;
using Taxi.Prism.Helpers;
using Taxi.Prism.Views;

namespace Taxi.Prism.ViewModels
{
    public class MyTripPageViewModel : ViewModelBase
    {
        private TripResponse _trip;
        private double _distance;
        private string _time;
        private string _value;

        public MyTripPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = Languages.MyTrip;
        }

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

        public TripResponse Trip 
        {
            get => _trip;
            set => SetProperty(ref _trip, value);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            Trip = parameters.GetValue<TripResponse>("trip");
            TripSummary tripSummary = GeoHelper.GetTripSummary(Trip);
            Distance = tripSummary.Distance;
            Time = $"{tripSummary.Time.ToString().Substring(0, 8)}";
            
            if (tripSummary.Value == 5600)
            {
                Value = $"{tripSummary.Value:C0}";
            }
            else
            {
                Value = $"{tripSummary.Value:C0}";
            }

            MyTripPage.GetInstance().DrawMap(Trip);
        }
    }
}
