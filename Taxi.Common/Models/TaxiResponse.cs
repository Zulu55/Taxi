using System.Collections.Generic;

namespace Taxi.Common.Models
{
    public class TaxiResponse
    {
        public int Id { get; set; }

        public string Plaque { get; set; }

        public List<TripResponse> Trips { get; set; }

        public UserResponse User { get; set; }
    }
}
