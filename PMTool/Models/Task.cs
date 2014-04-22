using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using System.ComponentModel;

namespace PMTool.Models
{

    public class Task
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TaskID { get; set; }

        [Required]
        [Display(Name = "Project Name")]
        public long ProjectID { get; set; }

        public virtual Project Project { get; set; }

        [Required]
        [Display(Name = "Title")]
        [MaxLength(75)]
        public string Title { get; set; }

        [AllowHtml]
        [Display(Name = "Task Description")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        
        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }

        
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        [Required]
        [Display(Name = "Estimated Task Hour")]
        public decimal TaskHour { get; set; }

        //[Required]
        public int? PriorityID { get; set; }

        public virtual Priority Priority { get; set; }


        [Required]
        public bool IsActive { get; set; }

        public long? ParentTaskId { get; set; }

        public virtual List<Task> ChildTask { get; set; }

        [ForeignKey("ParentTaskId")]
        public virtual Task ParentTask { get; set; }

        public string Attachments { get; set; }

        

        [Required]
        public DateTime CreateDate { get; set; }

        [Required]
        public DateTime ModificationDate { get; set; }

        [Required]
        public DateTime ActionDate { get; set; }

        public virtual List<UserProfile> Users { get; set; }

        public virtual List<UserProfile> Followers { get; set; }

        public virtual List<Label> Labels { get; set; }



        public List<string> SelectedAssignedUsers { get; set; }

        public List<string> SelectedFollowedUsers { get; set; }


        public List<string> SelectedLabels { get; set; }

        public virtual List<TimeLog> LoggedTime { get; set; }


        [Display(Name = "Status")]
        public string Status { get; set; }


        [Display(Name = "Status")]
        public long? ProjectStatusID { get; set; }


        public  string allStatus { get; set; }


        public virtual ProjectStatus ProjectStatus { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        [Required]
        public int ModifiedBy { get; set; }


        [ForeignKey("ModifiedBy")]
        public virtual UserProfile ModifiedByUser { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual UserProfile CreatedByUser { get; set; }


        [Display(Name = "Actual Task Hour")]
        public decimal ActualTaskHour { get; set; }


        public long? SprintID { get; set; }

        [ForeignKey("SprintID")]
        public virtual Sprint Sprint { get; set; }

        [Display(Name = "Estimated Preview Date")]
        [DefaultValue(typeof(DateTime), "2000-01-01")]
        public DateTime? EstimatedPreviewDate { get; set; }

        [Display(Name = "Actual Preview Date")]
        [DefaultValue(typeof(DateTime), "2000-01-01")]
        public DateTime? ActualPreviewDate { get; set; }

        [Display(Name = "Forecast Live Date")]
        [DefaultValue(typeof(DateTime), "2000-01-01")]
        public DateTime? ForecastLiveDate { get; set; }

        [Display(Name = "Actual Live Date")]
        [DefaultValue(typeof(DateTime), "2000-01-01")]
        public DateTime? ActualLiveDate { get; set; }


        public string TaskUID { get; set; }

        [NotMapped]
        public string GroupBy { get; set; }
    }
}