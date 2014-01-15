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
    public class ProjectRepository : IProjectRepository
    {
        PMToolContext context = new PMToolContext();

           public  ProjectRepository()
            : this(new PMToolContext())
        {
            
        }
           public ProjectRepository(PMToolContext context)
        {
            
            this.context = context;
        }

        public IQueryable<Project> All
        {
            get { return context.Projects; }
        }

        public IQueryable<Project> AllIncluding(params Expression<Func<Project, object>>[] includeProperties)
        {
            IQueryable<Project> query = context.Projects;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Project Find(long id)
        {
            return context.Projects.Find(id);
        }

        public void InsertOrUpdate(Project project)
        {
            if (project.ProjectID == default(long)) {
                // New entity
                context.Projects.Add(project);
            } else {
                // Existing entity
                context.Entry(project).State = System.Data.Entity.EntityState.Modified;
            }
        }

        public void Delete(long id)
        {
            var project = context.Projects.Find(id);
            context.Projects.Remove(project);
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

    public interface IProjectRepository : IDisposable
    {
        IQueryable<Project> All { get; }
        IQueryable<Project> AllIncluding(params Expression<Func<Project, object>>[] includeProperties);
        Project Find(long id);
        void InsertOrUpdate(Project project);
        void Delete(long id);
        void Save();
    }
}