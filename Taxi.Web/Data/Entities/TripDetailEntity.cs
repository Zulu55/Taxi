using System;
using System.ComponentModel.DataAnnotations;

namespace Taxi.Web.Data.Entities
{
    public class TripDetailEntity
    {
        public int Id { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Start Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm}", ApplyFormatInEditMode = false)]
        public DateTime Date { get; set; }

        public DateTime DateLocal => Date.ToLocalTime();

        [MaxLength(500, ErrorMessage = "The {0} field must have {1} characters.")]
        public string Address { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public TripEntity Trip { get; set; }
    }
}
