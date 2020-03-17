using System.ComponentModel.DataAnnotations;

namespace Taxi.Common.Models
{
    public class CompleteTripRequest
    {
        [Required]
        public int TripId { get; set; }

        [MaxLength(500, ErrorMessage = "The {0} field must have {1} characters.")]
        public string Target { get; set; }

        public double TargetLatitude { get; set; }

        public double TargetLongitude { get; set; }

        public float Qualification { get; set; }

        public string Remarks { get; set; }
    }
}
