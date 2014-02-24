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
    public class TaskActivityLogRepository : ITaskActivityLogRepository
    {
         PMToolContext context = new PMToolContext();


          public  TaskActivityLogRepository()
            : this(new PMToolContext())
        {
            
        }
          public TaskActivityLogRepository(PMToolContext context)
        {
            
            this.context = context;
        }

        public IQueryable<TaskActivityLog> All
        {
            get { return context.TaskActivityLogs; }
        }

        public IQueryable<TaskActivityLog> AllIncluding(params Expression<Func<TaskActivityLog, object>>[] includeProperties)
        {
            IQueryable<TaskActivityLog> query = context.TaskActivityLogs;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public TaskActivityLog Find(long id)
        {
            return context.TaskActivityLogs.Find(id);
        }

        public void InsertOrUpdate(TaskActivityLog taskactivitylog)
        {
            if (taskactivitylog.TaskActivityLogID == default(long)) {
                // New entity
                context.TaskActivityLogs.Add(taskactivitylog);
            } else {
                // Existing entity
                context.Entry(taskactivitylog).State = System.Data.Entity.EntityState.Modified;
            }
        }

        public void Delete(long id)
        {
            var taskactivitylog = context.TaskActivityLogs.Find(id);
            context.TaskActivityLogs.Remove(taskactivitylog);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Dispose() 
        {
            context.Dispose();
        }

        public List<TaskActivityLog> AllByTaskID(long taskID)
        {
            return context.TaskActivityLogs.Where(a => a.TaskID == taskID).OrderByDescending(a=>a.ModificationDate).ToList();
        }
    }

    public interface ITaskActivityLogRepository : IDisposable
    {
        IQueryable<TaskActivityLog> All { get; }
        IQueryable<TaskActivityLog> AllIncluding(params Expression<Func<TaskActivityLog, object>>[] includeProperties);
        TaskActivityLog Find(long id);
        void InsertOrUpdate(TaskActivityLog taskactivitylog);
        void Delete(long id);
        void Save();
    }
}