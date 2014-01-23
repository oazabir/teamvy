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


        public virtual Project Project { get; set; }

        [Required]
        public Guid UserID { get; set; }

        public virtual User User { get; set; }
    }
}