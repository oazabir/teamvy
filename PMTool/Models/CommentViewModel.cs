using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PMTool.Models
{
    public class CommentViewModel
    {
        public long ID { get; set; }

        public string Message { get; set; }

        public long TaskID { get; set; }
        public virtual Task Task { get; set; }

        public long? ParentComment { get; set; }

        public int CommentBy { get; set; }

        public virtual UserProfile CommentByUser { get; set; }

        public DateTime CreateDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public virtual List<Comment> ReplyComments { get; set; }

    }
}