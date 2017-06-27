using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TataApp.Domain
{
    public class Employee
    {
        [Key]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(50, ErrorMessage = "The maximun length for field {0} is {1} characters")]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(50, ErrorMessage = "The maximun length for field {0} is {1} characters")]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "Employee Code")]
        public int EmployeeCode { get; set; }

        [Display(Name = "Document Type")]
        public int DocumentTypeId { get; set; }

        [Display(Name = "Login Type")]
        public int LoginTypeId { get; set; }

        [MaxLength(20, ErrorMessage = "The maximun length for field {0} is {1} characters")]
        [Display(Name = "Document")]
        public string Document { get; set; }

        [DataType(DataType.ImageUrl)]
        public string Picture { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(100, ErrorMessage = "The maximun length for field {0} is {1} characters")]
        [DataType(DataType.EmailAddress)]
        [Index("User_Email_Index", IsUnique = true)]
        public string Email { get; set; }

        [MaxLength(20, ErrorMessage = "The maximun length for field {0} is {1} characters")]
        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [MaxLength(100, ErrorMessage = "The maximun length for field {0} is {1} characters")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Display(Name = "Employee")]
        public string FullName { get { return string.Format("{0} {1}", FirstName, LastName); } }

        [JsonIgnore]
        public virtual DocumentType DocumentType { get; set; }

        [JsonIgnore]
        public virtual LoginType LoginType { get; set; }

        [JsonIgnore]
        public virtual ICollection<Time> Times { get; set; }
    }
}