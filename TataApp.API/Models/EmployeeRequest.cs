namespace TataApp.API.Models
{
    using Domain;
    using System.ComponentModel.DataAnnotations.Schema;

    [NotMapped]
    public class EmployeeRequest : Employee
    {
        public string Password { get; set; }

        public byte[] ImageArray { get; set; }
    }
}