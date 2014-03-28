using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using PMTool.Models;
using PMTool.Repository;

namespace PMTool.Repository
{ 
    public class TaskMessageRepository : ITaskMessageRepository
    {
        PMToolContext context = new PMToolContext();

           public  TaskMessageRepository()
            : this(new PMToolContext())
        {
            
        }
           public TaskMessageRepository(PMToolContext context)
        {
            
            this.context = context;
        }
        public IQueryable<TaskMessage> All
        {
            get { return context.TaskMessages; }
        }

        public IQueryable<TaskMessage> AllIncluding(params Expression<Func<TaskMessage, object>>[] includeProperties)
        {
            IQueryable<TaskMessage> query = context.TaskMessages;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public TaskMessage Find(long id)
        {
            return context.TaskMessages.Find(id);
        }

        public void InsertOrUpdate(TaskMessage taskmessage)
        {
            if (taskmessage.TaskMessageID == default(long)) {
                // New entity
                context.TaskMessages.Add(taskmessage);
            } else {
                // Existing entity
                context.Entry(taskmessage).State = System.Data.Entity.EntityState.Modified;
            }
        }

        public void Delete(long id)
        {
            var taskmessage = context.TaskMessages.Find(id);
            context.TaskMessages.Remove(taskmessage);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Dispose() 
        {
            context.Dispose();
        }

        public List<TaskMessage> FindAllByTask(long taskID)
        {
            return context.TaskMessages.Where(t => t.TaskID == taskID).OrderByDescending(d=>d.CreateDate).ToList();
        }
    }

    public interface ITaskMessageRepository : IDisposable
    {
        IQueryable<TaskMessage> All { get; }
        IQueryable<TaskMessage> AllIncluding(params Expression<Func<TaskMessage, object>>[] includeProperties);
        TaskMessage Find(long id);
        void InsertOrUpdate(TaskMessage taskmessage);
        void Delete(long id);
        void Save();
        List<TaskMessage> FindAllByTask(long taskID);
    }
}