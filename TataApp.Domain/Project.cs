﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TataApp.Domain
{
    public class Project
    {
        [Key]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(50, ErrorMessage = "The maximun length for field {0} is {1} characters")]
        [Display(Name = "Description")]
        [Index("Project_Description_Index", IsUnique = true)]
        public string Description { get; set; }

        [JsonIgnore]
        public virtual ICollection<Time> Times { get; set; }
    }
}
