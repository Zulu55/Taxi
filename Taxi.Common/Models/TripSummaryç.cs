using System;

namespace Taxi.Common.Models
{
    public class TripSummary
    {
        public double Distance { get; set; }

        public TimeSpan Time { get; set; }

        public decimal Value { get; set; }
    }
}
