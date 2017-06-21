using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace TataApp.Domain
{
    public class Time
    {
        [Key]
        public int TimeId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Display(Name = "Project")]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Display(Name = "Activity")]
        public int ActivityId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Display(Name = "Date registered")]
        [DataType(DataType.Date)]
        public DateTime DateRegistered { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Display(Name = "Date reported")]
        [DataType(DataType.Date)]
        public DateTime DateReported { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Display(Name = "From")]
        [DataType(DataType.Time)]
        public DateTime From { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Display(Name = "To")]
        [DataType(DataType.Time)]
        public DateTime To { get; set; }

        [MaxLength(200, ErrorMessage = "The maximun length for field {0} is {1} characters")]
        [Display(Name = "Remarks")]
        [DataType(DataType.MultilineText)]
        public string Remarks { get; set; }

        [JsonIgnore]
        public virtual Employee Employee { get; set; }

        [JsonIgnore]
        public virtual Project Project { get; set; }

        [JsonIgnore]
        public virtual Activity Activity { get; set; }
    }
}