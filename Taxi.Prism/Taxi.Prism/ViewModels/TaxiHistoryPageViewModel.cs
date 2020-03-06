using Prism.Commands;
using Prism.Navigation;
using System.Collections.Generic;
using System.Linq;
using Taxi.Common.Models;
using Taxi.Common.Services;
using Taxi.Prism.Helpers;

namespace Taxi.Prism.ViewModels
{
    public class TaxiHistoryPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private TaxiResponse _taxi;
        private DelegateCommand _checkPlaqueCommand;
        private bool _isRunning;
        private List<TripItemViewModel> _trips;

        public TaxiHistoryPageViewModel(
            INavigationService navigationService,
            IApiService apiService) : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            Title = Languages.TaxiHistory;
        }

        public List<TripItemViewModel> Trips
        {
            get => _trips;
            set => SetProperty(ref _trips, value);
        }

        public TaxiResponse Taxi
        {
            get => _taxi;
            set => SetProperty(ref _taxi, value);
        }

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public string PlaqueLetters { get; set; }

        public int? PlaqueNumbers { get; set; }

        public DelegateCommand CheckPlaqueCommand => _checkPlaqueCommand ?? (_checkPlaqueCommand = new DelegateCommand(CheckPlaqueAsync));

        private async void CheckPlaqueAsync()
        {
            if (string.IsNullOrEmpty(PlaqueLetters) || PlaqueNumbers == 0)
            {
                await App.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.PlaqueError1,
                    Languages.Accept);
                return;
            }

            IsRunning = true;
            string url = App.Current.Resources["UrlAPI"].ToString();
            bool connection = await _apiService.CheckConnectionAsync(url);
            if (!connection)
            {
                IsRunning = false;
                await App.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.ConnectionError,
                    Languages.Accept);
                return;
            }

            Response response = await _apiService.GetTaxiAsync($"{PlaqueLetters}{PlaqueNumbers}", url, "api", "/Taxis");
            IsRunning = false;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    response.Message,
                    Languages.Accept);
                return;
            }

            Taxi = (TaxiResponse)response.Result;
            Trips = Taxi.Trips.Where(t => t.Qualification != 0).Select(t => new TripItemViewModel(_navigationService)
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
            }).OrderByDescending(t => t.StartDate).ToList();
        }
    }
}
