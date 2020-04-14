using System;
using System.Collections.Generic;
using System.Linq;
using Taxi.Common.Models;
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

        public static TripSummary GetTripSummary(TripResponse trip)
        {
            try
            {
                double distance = 0;

                if (trip.TripDetails == null || trip.TripDetails.Count < 2)
                {
                    return new TripSummary();
                }

                List<TripDetailResponse> details = trip.TripDetails.ToList();
                for (int i = 0; i < details.Count - 1; i++)
                {
                    Position a = new Position(details[i].Latitude, details[i].Longitude);
                    Position b = new Position(details[i + 1].Latitude, details[i + 1].Longitude);
                    distance += GetDistance(a, b, UnitOfLength.Kilometers) * 1000;
                }

                decimal value = (decimal)(3600 + Math.Truncate(distance / 78) * 110);

                return new TripSummary
                {
                    Distance = distance,
                    Time = details[details.Count - 1].Date.Subtract(details[0].Date),
                    Value = value < 5600 ? 5600 : value
                };

            }
            catch
            {
                return new TripSummary { Value = 5600 };
            }
        }
    }
}
