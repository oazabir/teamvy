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

        private List<User> userList = new List<User>();
        private List<Sprint> sprintList = new List<Sprint>();

        public List<Sprint> SprintList
        {
            get { return sprintList; }
            set { sprintList = value; }
        }

        public List<User> UserList
        {
            get { return userList; }
            set { userList = value; }
        }

        public long? SelectedStatusID { get; set; }
        public long? SelectedPriorityID { get; set; }
        public long? SelectedProjectID { get; set; }
        public Guid? SelectedUserID { get; set; }
        public long? SelectedSprintID { get; set; }

    }
}