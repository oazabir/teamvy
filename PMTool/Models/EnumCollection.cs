using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMTool.Models
{
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