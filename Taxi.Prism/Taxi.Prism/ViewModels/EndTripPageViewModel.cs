using Prism.Navigation;
using System;
using System.Collections.ObjectModel;
using Taxi.Common.Models;
using Taxi.Prism.Helpers;

namespace Taxi.Prism.ViewModels
{
    public class EndTripPageViewModel : ViewModelBase
    {
        private TripResponse _trip;
        private bool _isRunning;
        private bool _isEnabled;
        private float _qualification;
        private Comment _comment;
        private ObservableCollection<Comment> _comments;
        private string _remark;
        private double _distance;
        private DateTime _time;
        private decimal _value;

        public EndTripPageViewModel(INavigationService navigationService)
            : base(navigationService)
        {
            Title = Languages.EndTrip;
            IsEnabled = true;
            Comments = new ObservableCollection<Comment>(CombosHelper.GetComments());
        }

        public decimal Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        public double Distance
        {
            get => _distance;
            set => SetProperty(ref _distance, value);
        }

        public DateTime Time
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

            _trip = parameters.GetValue<TripResponse>("trip");
        }
    }
}
