using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace PMTool.Models
{
    public class ProjectStatus
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ProjectStatusID { get; set; }

        [Required]
        [Display(Name = "Project Name")]
        public long ProjectID { get; set; }


        [Required]
        [Display(Name = "Column Name")]
        public string Name { get; set; }


        public virtual Project Project { get; set; }

        [DefaultValue("#3B6998")]
        public string color { get; set; }
    }
}