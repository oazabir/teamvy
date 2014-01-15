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
    public class PriorityRepository : IPriorityRepository
    {
        PMToolContext context = new PMToolContext();

           public  PriorityRepository()
            : this(new PMToolContext())
        {
            
        }
           public PriorityRepository(PMToolContext context)
        {
            
            this.context = context;
        }

        public IQueryable<Priority> All
        {
            get { return context.Priorities; }
        }

        public IQueryable<Priority> AllIncluding(params Expression<Func<Priority, object>>[] includeProperties)
        {
            IQueryable<Priority> query = context.Priorities;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Priority Find(int id)
        {
            return context.Priorities.Find(id);
        }

        public void InsertOrUpdate(Priority priority)
        {
            if (priority.PriorityID == default(int)) {
                // New entity
                context.Priorities.Add(priority);
            } else {
                // Existing entity
                context.Entry(priority).State = System.Data.Entity.EntityState.Modified;
            }
        }

        public void Delete(int id)
        {
            var priority = context.Priorities.Find(id);
            context.Priorities.Remove(priority);
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

    public interface IPriorityRepository : IDisposable
    {
        IQueryable<Priority> All { get; }
        IQueryable<Priority> AllIncluding(params Expression<Func<Priority, object>>[] includeProperties);
        Priority Find(int id);
        void InsertOrUpdate(Priority priority);
        void Delete(int id);
        void Save();
    }
}