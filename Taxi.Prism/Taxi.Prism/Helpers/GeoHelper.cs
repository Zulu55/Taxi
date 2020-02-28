using System;
using Xamarin.Forms.Maps;

namespace Taxi.Prism.Helpers
{
    public static class GeoHelper
    {
        public static double GetDistance(Position baseCoordinates, Position targetCoordinates, UnitOfLength unitOfLength)
        {
            double baseRad = Math.PI * baseCoordinates.Latitude / 180;
            double targetRad = Math.PI * targetCoordinates.Latitude / 180;
            double theta = baseCoordinates.Longitude - targetCoordinates.Longitude;
            double thetaRad = Math.PI * theta / 180;

            double dist =
                Math.Sin(baseRad) * Math.Sin(targetRad) + Math.Cos(baseRad) *
                Math.Cos(targetRad) * Math.Cos(thetaRad);
            dist = Math.Acos(dist);

            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            return unitOfLength.ConvertFromMiles(dist);
        }
    }
}
