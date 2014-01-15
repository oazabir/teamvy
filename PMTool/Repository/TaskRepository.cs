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
    public class TaskRepository : ITaskRepository
    {
        PMToolContext context = new PMToolContext();

           public  TaskRepository()
            : this(new PMToolContext())
        {
            
        }
           public TaskRepository(PMToolContext context)
        {
            
            this.context = context;
        }

        public IQueryable<Task> All
        {
            get { return context.Tasks; }
        }

        public IQueryable<Task> AllIncluding(params Expression<Func<Task, object>>[] includeProperties)
        {
            IQueryable<Task> query = context.Tasks;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Task Find(long id)
        {
            return context.Tasks.Find(id);
        }

        public void InsertOrUpdate(Task task)
        {
            if (task.TaskID == default(long)) {
                // New entity
                context.Tasks.Add(task);
            } else {
                // Existing entity
                context.Entry(task).State = System.Data.Entity.EntityState.Modified;
            }
        }

        public void Delete(long id)
        {
            var task = context.Tasks.Find(id);
            context.Tasks.Remove(task);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Dispose() 
        {
            context.Dispose();
        }
    }

    public interface ITaskRepository : IDisposable
    {
        IQueryable<Task> All { get; }
        IQueryable<Task> AllIncluding(params Expression<Func<Task, object>>[] includeProperties);
        Task Find(long id);
        void InsertOrUpdate(Task task);
        void Delete(long id);
        void Save();
    }
}