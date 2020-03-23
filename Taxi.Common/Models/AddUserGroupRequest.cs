using System;
using System.ComponentModel.DataAnnotations;

namespace Taxi.Common.Models
{
    public class AddUserGroupRequest
    {
        [Required]
        public Guid UserId { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        public string CultureInfo { get; set; }
    }
}
