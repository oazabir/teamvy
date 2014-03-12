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
            List<Task> tasks=context.Tasks.Where(p=>p.ProjectID==ProjectID).ToList();
            var ProjectStatuses = context.ProjectStatuses.Where(p => p.ProjectID == ProjectID).ToList();
            foreach (ProjectStatus item in ProjectStatuses)
            {
                if (!tasks.Any(t => t.ProjectStatusID == item.ProjectStatusID))
                {

                    context.ProjectStatuses.Remove(item);
                }
            }
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


        public ProjectStatus FindbyProjectIDAndProjectStatusName(long projectID, string status)
        {
            return context.ProjectStatuses.Where(p => p.Name == status && p.ProjectID == projectID).FirstOrDefault();
        }


        public List<ProjectStatus> FindbyProjectID(long projectID)
        {
            return context.ProjectStatuses.Where(p => p.ProjectID == projectID).ToList();
        }

        public List<ProjectStatus> FindbyProjectIDWithoutUnmovable(long projectID)
        {
           return context.ProjectStatuses.Where(p => p.ProjectID == projectID && p.Name!="Closed").OrderBy(c=>c.SlNo).ThenBy(c=>c.ProjectStatusID).ToList();
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
        ProjectStatus FindbyProjectIDAndProjectStatusName(long projectID, string status);
        List<ProjectStatus> FindbyProjectID(long projectID);
        List<ProjectStatus> FindbyProjectIDWithoutUnmovable(long projectID);
        void Save();
    }
}