using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace PMTool.Models
{
    public class EmailScheduler
    {
        
        [Required,Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long SchedulerID { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }
        
        [Required]
        [Display(Name = "Project Name")]
        public long ProjectID { get; set; }

        [ForeignKey("ProjectID")]
        public virtual Project Project { get; set; }

        [Required]
        [Display(Name = "Schedule Type")]
        public int? ScheduleTypeID { get; set; } /* 1 = Daily, 2 = Weekly, 3 = Monthly */
        public virtual IEnumerable<SelectListItem> ScheduleType { get; set; }  
        //public virtual List<SelectList> ScheduleType { get; set; }
        //public virtual ScheduleType ScheduleType { get; set; }
 
        [Display(Name = "Time of Day")]
        public TimeSpan? ScheduledTime { get; set; }

        [Display(Name = "Recipient Users")]
        public int? RecipientUserType { get; set; } /* 1 = Task Users, 2 = Task Followers, 3 = Task Users and followers, 4 = Project Users */
        public virtual IEnumerable<SelectListItem> EmailRecipientUsers { get; set; } 

        //public virtual EmailRecipientUsers EmailRecipientUsers { get; set; }
        
        [Display(Name = "Day's Name")]
        public int? ScheduledDay { get; set; }  /* 1 = Saturday, 2 = Sunday, 3 = Monday */
        public virtual IEnumerable<SelectListItem> Days { get; set; }


        [Display(Name = "Date")]
        public DateTime? ScheduledDate { get; set; }

        public bool? IsActive { get; set; }


        [Required]
        public int CreatedBy { get; set; }

        [Required]
        public int? ModifiedBy { get; set; }


        [ForeignKey("ModifiedBy")]
        public virtual UserProfile ModifiedByUser { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual UserProfile CreatedByUser { get; set; }



        [Required]
        public DateTime CreateDate { get; set; }

        [Required]
        public DateTime? ModificationDate { get; set; }

    }
}