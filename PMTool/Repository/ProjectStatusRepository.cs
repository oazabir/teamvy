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
    public class ProjectStatusRepository : IProjectStatusRepository
    {
        PMToolContext context = new PMToolContext();
        
          public  ProjectStatusRepository()
            : this(new PMToolContext())
        {
            
        }
          public ProjectStatusRepository(PMToolContext context)
        {
            
            this.context = context;
        }

          public IQueryable<ProjectStatus> All
        {
            get { return context.ProjectStatuses; }
        }

          public IQueryable<ProjectStatus> AllIncluding(params Expression<Func<ProjectStatus, object>>[] includeProperties)
        {
            IQueryable<ProjectStatus> query = context.ProjectStatuses;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

          public ProjectStatus Find(long id)
        {
            return context.ProjectStatuses.Find(id);
        }

          public void InsertOrUpdate(ProjectStatus ProjectStatus)
        {
            if (ProjectStatus.ProjectStatusID == default(long)) {
                // New entity
                context.ProjectStatuses.Add(ProjectStatus);
            } else {
                // Existing entity
                context.Entry(ProjectStatus).State = System.Data.Entity.EntityState.Modified;
            }
        }

        public void Delete(long id)
        {
            var ProjectStatus = context.ProjectStatuses.Find(id);
            context.ProjectStatuses.Remove(ProjectStatus);
        }


        public void DeletebyProjectID(long ProjectID)
        {
            var ProjectStatus = context.ProjectStatuses.Where(p => p.ProjectID == ProjectID).ToList();
            context.ProjectStatuses.RemoveRange(ProjectStatus);
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
            var ProjectStatus = context.ProjectStatuses.Where(p => p.ProjectStatusID == status && p.ProjectID == projectID).FirstOrDefault();
            context.ProjectStatuses.Remove(ProjectStatus); 
        }

        public ProjectStatus FindbyProjectIDAndProjectStatusID(long projectID, long status)
        {
            return context.ProjectStatuses.Where(p => p.ProjectStatusID == status && p.ProjectID == projectID).FirstOrDefault();
        }
    }

    public interface IProjectStatusRepository : IDisposable
    {
        IQueryable<ProjectStatus> All { get; }
        IQueryable<ProjectStatus> AllIncluding(params Expression<Func<ProjectStatus, object>>[] includeProperties);
        ProjectStatus Find(long id);
        void InsertOrUpdate(ProjectStatus ProjectStatus);
        void Delete(long id);
        void DeleteByProjectIDAndColID(long status, long projectID);
        ProjectStatus FindbyProjectIDAndProjectStatusID(long projectID, long status);
        void Save();
    }
}