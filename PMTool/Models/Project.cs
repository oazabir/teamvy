using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMTool.Models
{
    public class Project
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ProjectID { get; set; }


        [Required]
        [Display(Name = "Project Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Project Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Project Is Locked")]
        public bool IsLocked { get; set; }

        [Required]
        [Display(Name = "Project Is Active")]
        public bool IsActive { get; set; }

        [Required]
        public Guid CreatedBy { get; set; }

        [Required]
        public Guid ModifieddBy { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        [Required]
        public DateTime ModificationDate { get; set; }

        [Required]
        public DateTime ActionDate { get; set; }

        public virtual User CreatedByUser { get; set; }
        public virtual User ModifiedByUser { get; set; }


        public virtual List<User> Users { get; set; }

    }
}