using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMTool.Models
{
    public class Mailer
    {
        string useMailID;

        public string UseMailID
        {
            get { return useMailID; }
            set { useMailID = value; }
        }

        string htmlMailBody;

        public string HtmlMailBody
        {
            get { return htmlMailBody; }
            set { htmlMailBody = value; }
        }
    }
}