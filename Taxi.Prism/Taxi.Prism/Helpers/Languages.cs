﻿using System.Globalization;
using Taxi.Prism.Interfaces;
using Taxi.Prism.Resources;
using Xamarin.Forms;

namespace Taxi.Prism.Helpers
{
    public static class Languages
    {
        static Languages()
        {
            CultureInfo ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            Resource.Culture = ci;
            Culture = ci.Name;
            DependencyService.Get<ILocalize>().SetLocale(ci);
        }

        public static string Culture { get; set; }

        public static string QualificationError => Resource.QualificationError;

        public static string DistanceTraveled => Resource.DistanceTraveled;

        public static string Time => Resource.Time;

        public static string ApproximateValue => Resource.ApproximateValue;

        public static string Comment => Resource.Comment;

        public static string Comment1 => Resource.Comment1;

        public static string Comment2 => Resource.Comment2;

        public static string Comment3 => Resource.Comment3;

        public static string Comment4 => Resource.Comment4;

        public static string Comment5 => Resource.Comment5;

        public static string Comment6 => Resource.Comment6;

        public static string GenericComment => Resource.GenericComment;

        public static string CommnetPlaceHolder => Resource.CommnetPlaceHolder;

        public static string GeolocationError => Resource.GeolocationError;

        public static string EndTrip => Resource.EndTrip;

        public static string Source => Resource.Source;

        public static string ConfirmAddress => Resource.ConfirmAddress;

        public static string ConfirmNewPassword => Resource.ConfirmNewPassword;

        public static string ConfirmNewPasswordError => Resource.ConfirmNewPasswordError;

        public static string ConfirmNewPasswordError2 => Resource.ConfirmNewPasswordError2;

        public static string ConfirmNewPasswordPlaceHolder => Resource.ConfirmNewPasswordPlaceHolder;

        public static string CurrentPassword => Resource.CurrentPassword;

        public static string CurrentPasswordError => Resource.CurrentPasswordError;

        public static string CurrentPasswordPlaceHolder => Resource.CurrentPasswordPlaceHolder;

        public static string NewPassword => Resource.NewPassword;

        public static string NewPasswordError => Resource.NewPasswordError;

        public static string NewPasswordPlaceHolder => Resource.NewPasswordPlaceHolder;

        public static string UserUpdated => Resource.UserUpdated;

        public static string Save => Resource.Save;

        public static string ChangePassword => Resource.ChangePassword;

        public static string PasswordRecover => Resource.PasswordRecover;

        public static string ForgotPassword => Resource.ForgotPassword;

        public static string PictureSource => Resource.PictureSource;

        public static string Cancel => Resource.Cancel;

        public static string FromCamera => Resource.FromCamera;

        public static string FromGallery => Resource.FromGallery;

        public static string Ok => Resource.Ok;

        public static string Address => Resource.Address;

        public static string AddressError => Resource.AddressError;

        public static string AddressPlaceHolder => Resource.AddressPlaceHolder;

        public static string Phone => Resource.Phone;

        public static string PhoneError => Resource.PhoneError;

        public static string PhonePlaceHolder => Resource.PhonePlaceHolder;

        public static string RegisterAs => Resource.RegisterAs;

        public static string RegisterAsError => Resource.RegisterAsError;

        public static string RegisterAsPlaceHolder => Resource.RegisterAsPlaceHolder;

        public static string PasswordConfirm => Resource.PasswordConfirm;

        public static string PasswordConfirmError1 => Resource.PasswordConfirmError1;

        public static string PasswordConfirmError2 => Resource.PasswordConfirmError2;

        public static string PasswordConfirmPlaceHolder => Resource.PasswordConfirmPlaceHolder;

        public static string User => Resource.User;

        public static string DocumentError => Resource.DocumentError;

        public static string FirstNameError => Resource.FirstNameError;

        public static string LastNameError => Resource.LastNameError;

        public static string Logout => Resource.Logout;

        public static string Email => Resource.Email;

        public static string LoginError => Resource.LoginError;

        public static string EmailPlaceHolder => Resource.EmailPlaceHolder;

        public static string EmailError => Resource.EmailError;

        public static string Password => Resource.Password;

        public static string PasswordError => Resource.PasswordError;

        public static string PasswordPlaceHolder => Resource.PasswordPlaceHolder;

        public static string Register => Resource.Register;

        public static string StartTrip => Resource.StartTrip;

        public static string NewTrip => Resource.NewTrip;

        public static string SeeTaxiHistory => Resource.SeeTaxiHistory;

        public static string AdminMyUserGroup => Resource.AdminMyUserGroup;

        public static string ModifyUser => Resource.ModifyUser;

        public static string ReportAnIncident => Resource.ReportAnIncident;

        public static string Login => Resource.LogIn;

        public static string Plaque => Resource.Plaque;

        public static string PlaquePlaceHolder => Resource.PlaquePlaceHolder;

        public static string CheckPlaque => Resource.CheckPlaque;

        public static string Qualification => Resource.Qualification;

        public static string NumberOfTrips => Resource.NumberOfTrips;

        public static string Driver => Resource.Driver;

        public static string StartDate => Resource.StartDate;

        public static string Score => Resource.Score;

        public static string Remarks => Resource.Remarks;

        public static string Loading => Resource.Loading;

        public static string Accept => Resource.Accept;

        public static string ConnectionError => Resource.ConnectionError;

        public static string Error => Resource.Error;

        public static string PlaqueError1 => Resource.PlaqueError1;

        public static string PlaqueError2 => Resource.PlaqueError2;

        public static string TaxiHistory => Resource.TaxiHistory;
    }
}
