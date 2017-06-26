namespace TataApp.Domain
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Location
    {
        [Key]
        public int LocationId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(50, ErrorMessage = "The maximun length for field {0} is {1} characters")]
        [Display(Name = "Description")]
        [Index("Location_Description_Index", IsUnique = true)]
        public string Description { get; set; }

        [MaxLength(100, ErrorMessage = "The maximun length for field {0} is {1} characters")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "Latitude")]
        public double Latitude { get; set; }

        [Display(Name = "Longitude")]
        public double Longitude { get; set; }
    }
}