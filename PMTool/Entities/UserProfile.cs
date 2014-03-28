using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using PMTool.Models;


    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }
        public string NewProp { get; set; }

        public virtual List<Project> Projects { get; set; }

        public virtual List<Task> Tasks { get; set; }

        public virtual List<Task> FollowerTasks { get; set; }

        //New added for Project Owner
        public virtual List<Project> OwnerProjects { get; set; }
    }


