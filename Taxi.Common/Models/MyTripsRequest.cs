using System;

namespace Taxi.Common.Models
{
    public class MyTripsRequest
    {
        public string UserId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
