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

        DateTime? fromEndDate;

        public DateTime? FromEndDate
        {
          get { return fromEndDate; }
          set { fromEndDate = value; }
        }

        DateTime? toEndDate;

        public DateTime? ToEndDate
        {
            get { return toEndDate; }
            set { toEndDate = value; }
        }

        DateTime? fromSatrtDate;

        public DateTime? FromSatrtDate
        {
            get { return fromSatrtDate; }
            set { fromSatrtDate = value; }
        }

        DateTime? toSatrtDate;

        public DateTime? ToSatrtDate
        {
            get { return toSatrtDate; }
            set { toSatrtDate = value; }
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

        string fromSattus;

        public string FromSataus
        {
            get { return fromSattus; }
            set { fromSattus = value; }
        }

        string toStatus;    

        public string ToStatus
        {
            get { return toStatus; }
            set { toStatus = value; }
        }
    }
}