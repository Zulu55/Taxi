using System.ComponentModel.DataAnnotations;

namespace Taxi.Web.Models
{
    public class RecoverPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
