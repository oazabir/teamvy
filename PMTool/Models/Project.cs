﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMTool.Models
{
    /// <summary>
    /// Project Model
    /// </summary>
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
        public DateTime CreateDate { get; set; }

        [Required]
        public DateTime ModificationDate { get; set; }

        [Required]
        public DateTime ActionDate { get; set; }

       


        public virtual List<UserProfile> Users { get; set; }

        public virtual List<UserProfile> ProjectOwners { get; set; }
        public List<string> SelectedProjectsOwners { get; set; }


        public List<string> SelectedAssignedUsers { get; set; }



        public string allStatus { get; set; }

        public virtual List<ProjectStatus> ProjectStatuses { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        [Required]
        public int ModifiedBy { get; set; }


        [ForeignKey("ModifiedBy")]
        public virtual UserProfile ModifiedByUser { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual UserProfile CreatedByUser { get; set; }

        public virtual List<Sprint> Sprints { get; set; }

        public virtual List<Task> Tasks { get; set; }


    }
}