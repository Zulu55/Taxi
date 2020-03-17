using Prism.Navigation;
using Taxi.Prism.Helpers;
using Xamarin.Forms;

namespace Taxi.Prism.ViewModels
{
    public class HomePageViewModel : ViewModelBase
    {
        private static HomePageViewModel _instance;

        public HomePageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            _instance = this;
            Title = "Taxi Qualifier";
        }

        public static HomePageViewModel GetInstance()
        {
            return _instance;
        }

        public void AddMessage(string message)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                App.Current.MainPage.DisplayAlert(Languages.Error, message, Languages.Accept);
            });
        }
    }
}
