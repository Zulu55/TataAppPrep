using System;
using TataApp.Domain;

namespace TataApp.API.Models
{
    public class TimeResponse
    {
        public int TimeId { get; set; }

        public int EmployeeId { get; set; }

        public int ProjectId { get; set; }

        public int ActivityId { get; set; }

        public DateTime DateRegistered { get; set; }

        public DateTime DateReported { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public string Remarks { get; set; }

        public Project Project { get; set; }

        public Activity Activity { get; set; }
    }
}