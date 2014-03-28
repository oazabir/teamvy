using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PMTool.Models
{
    /*For time log entry against Task. Created by Mahedee @ 13-03-14*/

    public class TimeLog
    {

        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long LogID { get; set; }


        [Required]
        [Display(Name = "Task Title")]
        public long? SprintID { get; set; }
        public virtual Sprint Sprint { get; set; }

        [Required]
        [Display(Name = "Task Title")]
        public long? TaskID { get; set; }
        public virtual Task Task { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public virtual UserProfile User { get; set; }

        [Required]
        [Display(Name="Day")]
        //[DefaultValue(typeof(DateTime), "2000-01-01")]
        public DateTime? EntryDate { get; set; } //Entry Date

        [Required]
        [Display(Name = "Value")]
        [DefaultValue(0.0)]
        public decimal TaskHour { get; set; } //Task Hour value

        [Display(Name = "Summary")]
        public string Description { get; set; }


        [Required]
        public int CreatedBy { get; set; }

        [Required]
        public int ModifiedBy { get; set; }


        [ForeignKey("ModifiedBy")]
        public virtual UserProfile ModifiedByUser { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual UserProfile CreatedByUser { get; set; }
        


        [Required]
        public DateTime CreateDate { get; set; }

        [Required]
        public DateTime ModificationDate { get; set; }

        [Required]
        public DateTime ActionDate { get; set; }

    }
}