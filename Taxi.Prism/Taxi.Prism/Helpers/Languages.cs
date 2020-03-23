using System.Globalization;
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
