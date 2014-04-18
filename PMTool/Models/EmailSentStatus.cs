using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PMTool.Models
{
    public class EmailSentStatus
    {
        [Required,Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        
        /*PK = EmailSchedulerID*/
        public long EmailSchedulerID { get; set; }

        public int? ScheduleTypeID { get; set; } // Daily = 1, Weekly = 2, Monthly = 3

        public DateTime? ScheduleDateTime { get; set; }
        public DateTime? SentDateTime { get; set; }


        //public TimeSpan? ScheduleTime { get; set; } //Schedule Time

        //public TimeSpan? SentTime { get; set; } //Actual sent time

        public bool? SentStatus { get; set; }

        public DateTime? ActionTime { get; set; }

    }
}