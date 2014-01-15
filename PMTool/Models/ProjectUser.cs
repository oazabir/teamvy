using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMTool.Models
{
    public class ProjectUser
    {
        public long ProjectID { get; set; }
        public Guid UserID { get; set; }

        public virtual Project Project { get; set; }
        public virtual User User { get; set; }
    }
}