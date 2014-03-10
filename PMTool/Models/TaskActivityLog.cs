using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMTool.Models
{
    public class TaskActivityLog
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long TaskActivityLogID { get; set; }


        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "Choose File")]
        public string FileUrl { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }


        public string FileDisplayName { get; set; }

        [Required]
        public DateTime ModificationDate { get; set; }

        public long TaskID { get; set; }

        [NotMapped]
        public HttpPostedFileBase AtachFile { get; set; }

        public virtual Task Task { get; set; }

    }
}