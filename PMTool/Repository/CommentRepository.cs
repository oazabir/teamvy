using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using PMTool.Repository;
using PMTool.Models;

namespace PMTool.Repository
{ 
    public class CommentRepository : ICommentRepository
    {
        //PMToolContext context = new PMToolContext();
        
        PMToolContext context = new PMToolContext();

        public CommentRepository()
            : this(new PMToolContext())
        {

        }

        public CommentRepository(PMToolContext context)
        {
            this.context = context;
        }


        public IQueryable<Comment> All
        {
            get { return context.Comments; }
        }

        public IQueryable<Comment> AllIncluding(params Expression<Func<Comment, object>>[] includeProperties)
        {
            IQueryable<Comment> query = context.Comments;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Comment Find(long id)
        {
            return context.Comments.Find(id);
        }

        public List<Comment> GetComments(long taskId)
        {
            List<Comment> comments = context.Comments.Where(cmt => cmt.TaskID == taskId && cmt.ParentComment == null).ToList(); //&& t.ParentTaskId == null -- add later
            return comments;
  
            //IQueryable<Task> query = context.Tasks.Where(t => t.ProjectID == projectID && t.ParentTaskId == null);
            //foreach (var includeProperty in includeProperties)
            //{
            //    query = query.Include(includeProperty);

            //}
            //return query;
            
        }

        public void InsertOrUpdate(Comment comment)
        {
            if (comment.ID == default(long)) {
                // New entity
                context.Comments.Add(comment);
            } else {
                // Existing entity
                context.Entry(comment).State = System.Data.Entity.EntityState.Modified;
            }
        }

        public void Delete(long id)
        {
            var comment = context.Comments.Find(id);
            context.Comments.Remove(comment);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Dispose() 
        {
            context.Dispose();
        }

        public  List<Comment> GetReplyCommentsByID(long ID)
        {
            return context.Comments.Where(c => c.ParentComment == ID).ToList();
        }
    }

    public interface ICommentRepository : IDisposable
    {
        IQueryable<Comment> All { get; }
        IQueryable<Comment> AllIncluding(params Expression<Func<Comment, object>>[] includeProperties);
        Comment Find(long id);
        void InsertOrUpdate(Comment comment);
        void Delete(long id);
        void Save();
    }
}