using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMTool.Models
{
    public class Sprint
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SprintID { get; set; }


        [Required]
        [Display(Name = "Priority Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Priority Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Project Name")]
        public long ProjectID { get; set; }

        public virtual Project Project { get; set; }

        [Required]
        public bool IsActive { get; set; }


        public virtual List<Task> Tasks { get; set; }
    }
}