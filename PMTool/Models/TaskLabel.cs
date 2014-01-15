using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PMTool.Models
{
    public class TaskLabel
    {

         public long TaskID { get; set; }
         public long LabelID { get; set; }

         public virtual Task Task { get; set; }
         public virtual Label Label { get; set; }
    }
}