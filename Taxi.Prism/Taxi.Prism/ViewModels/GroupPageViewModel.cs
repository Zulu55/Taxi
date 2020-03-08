using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Taxi.Common.Helpers;
using Taxi.Common.Models;
using Taxi.Common.Services;
using Taxi.Prism.Helpers;
using Taxi.Prism.Views;

namespace Taxi.Prism.ViewModels
{
    public class GroupPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;
        private bool _isRunning;
        private ObservableCollection<UserGroupDetailItemViewModel> _users;
        private DelegateCommand _addUserCommand;

        public GroupPageViewModel(INavigationService navigationService, IApiService apiService)
            : base(navigationService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            Title = Languages.AdminMyUserGroup;
            LoadUsersAsync();
        }

        public DelegateCommand AddUserCommand => _addUserCommand ?? (_addUserCommand = new DelegateCommand(AddUserAsync));

        public bool IsRunning
        {
            get => _isRunning;
            set => SetProperty(ref _isRunning, value);
        }

        public ObservableCollection<UserGroupDetailItemViewModel> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        private async void AddUserAsync()
        {
            await _navigationService.NavigateAsync(nameof(AddUserToGroupPage));
        }

        private async void LoadUsersAsync()
        {
            IsRunning = true;
            string url = App.Current.Resources["UrlAPI"].ToString();
            bool connection = await _apiService.CheckConnectionAsync(url);
            if (!connection)
            {
                IsRunning = false;
                await App.Current.MainPage.DisplayAlert(Languages.Error, Languages.ConnectionError, Languages.Accept);
                return;
            }

            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(Settings.Token);
            UserResponse user = JsonConvert.DeserializeObject<UserResponse>(Settings.User);
            Response response = await _apiService.GetListAsync<UserGroupDetailResponse>(url, "/api", $"/UserGroups/{user.Id}", "bearer", token.Token);
            IsRunning = false;

            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(Languages.Error, response.Message, Languages.Accept);
                return;
            }

            if (response.Result != null)
            {
                List<UserGroupDetailResponse> users = (List<UserGroupDetailResponse>)response.Result;
                Users = new ObservableCollection<UserGroupDetailItemViewModel>(users.Select(u => new UserGroupDetailItemViewModel(_navigationService)
                {
                    Id = u.Id,
                    User = u.User
                }).ToList());
            }
        }
    }
}
