using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMTool.Models
{
    /// <summary>
    /// Sprint Model
    /// </summary>
    public class Sprint
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SprintID { get; set; }


        [Required]
        [Display(Name = "Sprint Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Sprint Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Project Name")]
        public long ProjectID { get; set; }

        public virtual Project Project { get; set; }

        [Required]
        public bool IsActive { get; set; }


        public virtual List<Task> Tasks { get; set; }

        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
    }
}