using Prism.Commands;
using Prism.Navigation;
using System.Threading.Tasks;
using Taxi.Prism.Helpers;

namespace Taxi.Prism.ViewModels
{
    public class ReportPageViewModel : ViewModelBase
    {
        private bool _isRunning;
        private bool _isEnabled;
        private DelegateCommand _reportCommand;

        public ReportPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = Languages.ReportAnIncident;
            IsEnabled = true;
        }

        public DelegateCommand ReportCommand => _reportCommand ?? (_reportCommand = new DelegateCommand(ReportAsync));

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

        public string PlaqueLetters { get; set; }

        public int? PlaqueNumbers { get; set; }

        public string Remark { get; set; }

        private async void ReportAsync()
        {
            bool isValid = await ValidateDataAsync();
            if (!isValid)
            {
                return;
            }
        }

        private async Task<bool> ValidateDataAsync()
        {
            if (string.IsNullOrEmpty(PlaqueLetters) || PlaqueNumbers == 0)
            {
                await App.Current.MainPage.DisplayAlert(Languages.Error, Languages.PlaqueError1, Languages.Accept);
                return false;
            }

            if (string.IsNullOrEmpty(Remark))
            {
                await App.Current.MainPage.DisplayAlert(Languages.Error, Languages.RemarksError, Languages.Accept);
                return false;
            }

            return true;
        }
    }
}
