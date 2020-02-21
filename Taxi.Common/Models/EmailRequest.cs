using System.ComponentModel.DataAnnotations;

namespace Taxi.Common.Models
{
    public class EmailRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}

