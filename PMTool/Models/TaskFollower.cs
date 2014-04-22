using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMTool.Models
{
    public class TaskFollower
    {
        public long TaskID { get; set; }
        public long FollowerID { get; set; }

        public virtual Task Task { get; set; }
        public virtual UserProfile Follower { get; set; }
    }
}