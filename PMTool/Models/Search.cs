using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMTool.Models
{
    public class Search
    {
        public List<ProjectStatus> statusList { get; set; }
        public List<Priority> priorityList { get; set; }
        public long? SelectedStatusID { get; set; }
        public long? SelectedPriorityID { get; set; }
        public long? SelectedProjectID { get; set; }

    }
}