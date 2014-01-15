using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMTool.Models
{
    public class Label
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long LabelID { get; set; }


        [Required]
        [Display(Name = "Label Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Label Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Label Is Active")]
        public bool IsActive { get; set; }

        [Required]
        public Guid CreatedBy { get; set; }

        public virtual User CreatedByUser { get; set; }

        [Required]
        public Guid ModifieddBy { get; set; }

        public virtual User ModifiedByUser { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        [Required]
        public DateTime ModificationDate { get; set; }

        [Required]
        public DateTime ActionDate { get; set; }

        public virtual List<Task> Tasks { get; set; }
    }
}