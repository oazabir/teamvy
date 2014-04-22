using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PMTool.Models
{
    public class Comment
    {
        [Required, Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Required]
        public string Message { get; set; }

        public long TaskID { get; set; }
        public virtual Task Task { get; set; }

        public long? ParentComment { get; set; }

        public int CommentBy { get; set; }

        [ForeignKey("CommentBy")]
        public virtual UserProfile CommentByUser { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

    }
}