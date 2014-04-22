using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMTool.Models
{
    public class Notification
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long NotificationID { get; set; }


        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public bool IsNoticed { get; set; }


        public long? TaskID { get; set; }


        public long? ProjectID { get; set; }


        public virtual Task Task { get; set; }


        [Required]
        public int UserID { get; set; }

        public virtual UserProfile User { get; set; }
    }
}