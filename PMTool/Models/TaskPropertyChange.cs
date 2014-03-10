using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMTool.Models
{
    public class TaskPropertyChange
    {
        bool isSatausChanged=false;
        bool isStartDateChanged = false;
        bool isEndtDateChanged = false;

        public bool IsEndtDateChanged
        {
            get { return isEndtDateChanged; }
            set { isEndtDateChanged = value; }
        }

        public bool IsStartDateChanged
        {
            get { return isStartDateChanged; }
            set { isStartDateChanged = value; }
        }

        public bool IsSatausChanged
        {
            get { return isSatausChanged; }
            set { isSatausChanged = value; }
        }
    }
}