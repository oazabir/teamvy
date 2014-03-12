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
    public class ProjectStatusRuleRepository : IProjectStatusRuleRepository
    {
        PMToolContext context = new PMToolContext();


          public  ProjectStatusRuleRepository()
            : this(new PMToolContext())
        {
            
        }
          public ProjectStatusRuleRepository(PMToolContext context)
        {
            
            this.context = context;
        }


        public IQueryable<ProjectStatusRule> All
        {
            get { return context.ProjectStatusRules; }
        }

        public IQueryable<ProjectStatusRule> AllIncluding(params Expression<Func<ProjectStatusRule, object>>[] includeProperties)
        {
            IQueryable<ProjectStatusRule> query = context.ProjectStatusRules;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public ProjectStatusRule Find(long id)
        {
            return context.ProjectStatusRules.Find(id);
        }

        public void InsertOrUpdate(ProjectStatusRule projectstatusrule)
        {
            if (projectstatusrule.ProjectStatusRuleID == default(long)) {
                // New entity
                context.ProjectStatusRules.Add(projectstatusrule);
            } else {
                // Existing entity
                context.Entry(projectstatusrule).State = System.Data.Entity.EntityState.Modified;
            }
        }

        public void Delete(long id)
        {
            var projectstatusrule = context.ProjectStatusRules.Find(id);
            context.ProjectStatusRules.Remove(projectstatusrule);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Dispose() 
        {
            context.Dispose();
        }

        public List<ProjectStatusRule> FindbyProjectID(long projectID)
        {
            return context.ProjectStatusRules.Where(r => r.ProjectID == projectID).ToList();
        }
    }

    public interface IProjectStatusRuleRepository : IDisposable
    {
        IQueryable<ProjectStatusRule> All { get; }
        IQueryable<ProjectStatusRule> AllIncluding(params Expression<Func<ProjectStatusRule, object>>[] includeProperties);
        ProjectStatusRule Find(long id);
        void InsertOrUpdate(ProjectStatusRule projectstatusrule);
        List<ProjectStatusRule> FindbyProjectID(long projectID);
        void Delete(long id);
        void Save();
    }
}