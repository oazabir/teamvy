using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMTool.Models
{
    public class TaskUser
    {
        public long TaskID { get; set; }
        public Guid UserID { get; set; }

        public virtual Task Task { get; set; }
        public virtual User User { get; set; }
    }
}