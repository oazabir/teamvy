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

    public class TaskMessage
    {

        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TaskMessageID { get; set; }



        [Required]
        [Display(Name = "Task Title")]
        public long TaskID { get; set; }
        public virtual Task Task { get; set; }

        [Required]
        [Display(Name = "From User Name")]
        public long FormUserID { get; set; }

        [ForeignKey("FormUserID")]
        public virtual UserProfile FromUser { get; set; }

        [Required]
        [Display(Name = "To User Name")]
        public long ToUserID { get; set; }

        [ForeignKey("ToUserID")]
        public virtual UserProfile ToUser { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }


        [Required]
        public string Message { get; set; }


        public List<string> SelectedToUsers { get; set; }
    }
}