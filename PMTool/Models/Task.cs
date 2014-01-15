using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

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
        public string Title { get; set; }

        [Required]
        [Display(Name = "Task Description")]
        public string Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public decimal TaskHour { get; set; }

        [Required]
        public int PriorityID { get; set; }

        public virtual Priority Priority { get; set; }

        [Required]
        [Display(Name = "Project Is Locked")]
        public bool IsLocked { get; set; }

        [Required]
        [Display(Name = "Project Is Active")]
        public bool IsActive { get; set; }

        public int? ParentTaskId { get; set; }

        public virtual List<Task> ChildTask { get; set; }

        public string Attachments { get; set; }

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

        public virtual List<User> Users { get; set; }

        public virtual List<User> Followers { get; set; }

        public virtual List<Label> Labels { get; set; }


        public virtual long UserID { get; set; }

        public List<string> SelectedAssignedUsers { get; set; }

        public List<string> SelectedFollowedUsers { get; set; }

        public string[] UserList { get; set; }

        public string[] FollowerList { get; set; }
    }
}