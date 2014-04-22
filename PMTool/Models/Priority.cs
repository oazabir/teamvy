using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMTool.Models
{
    public class Priority
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PriorityID { get; set; }


        [Required]
        [Display(Name = "Priority Name")]
        public string Name { get; set; }

        [Required] 
        [Display(Name = "Priority Description")]
        public string Description { get; set; }

    }
}