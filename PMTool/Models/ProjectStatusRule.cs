using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using System.Web.Mvc;

namespace PMTool.Models
{
    public class ProjectStatusRule
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ProjectStatusRuleID { get; set; }

        [Required]
        [Display(Name = "Project Name")]
        public long ProjectID { get; set; }


        [Required]
        [Display(Name = "Update on Change")]
        public DateMaper DateMaper { get; set; }


        [Required]
        [Display(Name = "Status")]
        public long ProjectStatusID { get; set; }

        [ForeignKey("ProjectStatusID")]
        public virtual ProjectStatus ProjectStatus { get; set; }


        public virtual Project Project { get; set; }


        public List<DateMaper> DateMaperSelectList = Enum.GetValues(typeof(DateMaper)).Cast<DateMaper>().Cast<DateMaper>().ToList();
    }


    /// <summary>
    /// Enum Name must be the same name of the task proerty 
    /// that you want to map with and also separeate the words
    /// by a "_" which will replace by an empty space when showing in dropwown list.
    /// </summary>
    public enum DateMaper
    {
        Estimated_Preview_Date=1,
        Actual_Preview_Date,
        Forecast_Live_Date,
        Actual_Live_Date
    }
}