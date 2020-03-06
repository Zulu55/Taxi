using System;
using System.ComponentModel.DataAnnotations;

namespace Taxi.Common.Models
{
    public class TripRequest
    {
        [RegularExpression(@"^([A-Za-z]{3}\d{3})$", ErrorMessage = "The field {0} must have three characters and three numbers.")]
        [Required(ErrorMessage = "The field {0} is mandatory.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "The {0} field must have {1} characters.")]
        public string Plaque { get; set; }

        [Required(ErrorMessage = "The field {0} is mandatory.")]
        public Guid UserId { get; set; }

        public string Address { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }
    }
}
