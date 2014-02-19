using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using PMTool.Repository;

namespace PMTool.Models
{ 
    public class SprintRepository : ISprintRepository
    {
       PMToolContext context = new PMToolContext();
        
          public  SprintRepository()
            : this(new PMToolContext())
        {
            
        }
          public SprintRepository(PMToolContext context)
        {
            
            this.context = context;
        }

        public IQueryable<Sprint> All
        {
            get { return context.Sprints; }
        }

        public IQueryable<Sprint> AllIncluding(params Expression<Func<Sprint, object>>[] includeProperties)
        {
            IQueryable<Sprint> query = context.Sprints;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Sprint Find(long id)
        {
            return context.Sprints.Find(id);
        }

        public void InsertOrUpdate(Sprint sprint)
        {
            if (sprint.SprintID == default(long)) {
                // New entity
                context.Sprints.Add(sprint);
            } else {
                // Existing entity
                context.Entry(sprint).State = System.Data.Entity.EntityState.Modified;
            }
        }

        public void Delete(long id)
        {
            var sprint = context.Sprints.Find(id);
            context.Sprints.Remove(sprint);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Dispose() 
        {
            context.Dispose();
        }

        public List<Sprint> AllByProjectID(long projectID)
        {
            return context.Sprints.Where(s => s.ProjectID == projectID).ToList();
        }
    }

    public interface ISprintRepository : IDisposable
    {
        IQueryable<Sprint> All { get; }
        IQueryable<Sprint> AllIncluding(params Expression<Func<Sprint, object>>[] includeProperties);
        Sprint Find(long id);
        void InsertOrUpdate(Sprint sprint);
        void Delete(long id);
        void Save();
        List<Sprint> AllByProjectID(long projectID);
    }
}