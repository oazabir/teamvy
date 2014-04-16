using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace PMTool.Models
{
    public enum ScheduleType
    {
        Daily = 1,
        Weekly = 2,
        Monthly = 3
    }


    public enum EmailRecipientUsers
    {
        [Description("Task's Users")]
        TaskUsers = 1,

        [Description("Task's Followers")]
        TaskFollowers = 2,

        [Description("Task's Users and followers")]
        TaskUsersnFollowers = 3,

        [Description("Projects's Users")]
        ProjectUsers = 4
    }

    public enum Week
    {
        Saturday = 1,
        Sunday = 2,
        Monday = 3,
        Tuesday = 4,
        Wednesday = 5,
        Thursday = 6,
        Friday = 7

    }

    public class EnumCollection
    {
        public enum TaskStatus
        {
            Open=1,
            In_Progress = 2,
            Pending=3,
            Closed =4,
            Reopened=5
        }





    }
}