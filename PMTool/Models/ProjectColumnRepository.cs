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
    public class ProjectColumnRepository : IProjectColumnRepository
    {
        PMToolContext context = new PMToolContext();
        
          public  ProjectColumnRepository()
            : this(new PMToolContext())
        {
            
        }
          public ProjectColumnRepository(PMToolContext context)
        {
            
            this.context = context;
        }

        public IQueryable<ProjectColumn> All
        {
            get { return context.ProjectColumns; }
        }

        public IQueryable<ProjectColumn> AllIncluding(params Expression<Func<ProjectColumn, object>>[] includeProperties)
        {
            IQueryable<ProjectColumn> query = context.ProjectColumns;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public ProjectColumn Find(long id)
        {
            return context.ProjectColumns.Find(id);
        }

        public void InsertOrUpdate(ProjectColumn projectcolumn)
        {
            if (projectcolumn.ProjectColumnID == default(long)) {
                // New entity
                context.ProjectColumns.Add(projectcolumn);
            } else {
                // Existing entity
                context.Entry(projectcolumn).State = System.Data.Entity.EntityState.Modified;
            }
        }

        public void Delete(long id)
        {
            var projectcolumn = context.ProjectColumns.Find(id);
            context.ProjectColumns.Remove(projectcolumn);
        }


        public void DeletebyProjectID(long ProjectID)
        {
            var projectcolumn = context.ProjectColumns.Where(p=>p.ProjectID==ProjectID).ToList();
             context.ProjectColumns.RemoveRange(projectcolumn);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Dispose() 
        {
            context.Dispose();
        }

        public void DeleteByProjectIDAndColID(long status, long projectID)
        {
            var projectcolumn = context.ProjectColumns.Where(p=>p.ProjectColumnID==status && p.ProjectID==projectID).FirstOrDefault();
            context.ProjectColumns.Remove(projectcolumn); 
        }

        public ProjectColumn FindbyProjectIDAndProjectColumnID(long projectID, long status)
        {
            return context.ProjectColumns.Where(p => p.ProjectColumnID == status && p.ProjectID == projectID).FirstOrDefault();
        }
    }

    public interface IProjectColumnRepository : IDisposable
    {
        IQueryable<ProjectColumn> All { get; }
        IQueryable<ProjectColumn> AllIncluding(params Expression<Func<ProjectColumn, object>>[] includeProperties);
        ProjectColumn Find(long id);
        void InsertOrUpdate(ProjectColumn projectcolumn);
        void Delete(long id);
        void DeleteByProjectIDAndColID(long status, long projectID);
        ProjectColumn FindbyProjectIDAndProjectColumnID(long projectID, long status);
        void Save();
    }
}