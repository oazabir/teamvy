using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Security;
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

        public IQueryable<Project> AllbyUserIncluding(Guid userID,params Expression<Func<Project, object>>[] includeProperties)
        {
            IQueryable<Project> query = context.Projects.Where(p => p.CreatedBy== userID);
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Project FindIncludingProjectStatus(long projectID)
        {

            Project project = context.Projects.Where(p => p.ProjectID == projectID).Include("ProjectStatuses").FirstOrDefault();

            return project;
        }

        //public IQueryable<Project> AllbyOwnerIncluding(Guid ownerId, params Expression<Func<Project, object>>[] includeProperties)
        public List<Project> AllbyOwnerIncluding(Guid ownerId, params Expression<Func<Project, object>>[] includeProperties)
        {
            //List<Task> taskList = context.Tasks.Include("Users").Where(t => t.Users.Any(u => u.UserId == user.UserId)).ToList();
            List<Project> projectList = context.Projects.Include("ProjectOwners").Where(t => t.ProjectOwners.Any(u => u.UserId == ownerId)).ToList();
            //IQueryable<Project> query = context.Projects.Where(p => p.CreatedBy == ownerId);
            //foreach (var includeProperty in includeProperties)
            //{
            //    query = query.Include(includeProperty);
            //}
            //return query;

            return projectList;

        }

        public Project Find(long id)
        {
            return context.Projects.Include("Users").Where(p=>p.ProjectID==id).FirstOrDefault();
        }

        public List<User> InsertOrUpdate(Project project)
        {
            List<User> userList = new List<User>();
            List<User> ownerList = new List<User>();

            if (project.ProjectID == default(long)) {
                // New entity
                context.Projects.Add(project);
            } else {
                // Existing entity
                var existingProject = context.Projects.Include("Users")
                    .Where(t => t.ProjectID == project.ProjectID).FirstOrDefault<Project>();

                userList = UpdateAssignedUsers(project, existingProject);
                ownerList = UpdateProjectOwners(project, existingProject);
                context.Entry(existingProject).CurrentValues.SetValues(project);
            }
            return userList;
        }

        private List<User> UpdateAssignedUsers(Project project, Project existingProject)
        {
            List<User> userList = new List<User>();
            var deletedUsers = existingProject.Users.ToList<User>();
            var addedUsers = project.Users.ToList<User>();
           // deletedUsers.ForEach(c => existingProject.Users.Remove(c));
            foreach (User user in existingProject.Users.ToList())
            {
                List<Task> taskList = context.Tasks.Include("Users").Where(t => t.Users.Any(u=>u.UserId==user.UserId)).ToList();
               if (taskList.Count == 0)
                   existingProject.Users.Remove(user);
               else
                   userList.Add(user);
            }
            foreach (User c in addedUsers)
            {

                if (context.Entry(c).State == System.Data.Entity.EntityState.Detached)
                    context.Users.Attach(c);
                existingProject.Users.Add(c);
            }
            return userList;
        }


        private List<User> UpdateProjectOwners(Project project, Project existingProject)
        {
            List<User> ownerList = new List<User>();
            var deletedOwners = existingProject.ProjectOwners.ToList<User>();
            var addedOwners = project.ProjectOwners.ToList<User>();
            // deletedUsers.ForEach(c => existingProject.Users.Remove(c));
            foreach (User owner in existingProject.ProjectOwners.ToList())
            {
                //List<Task> taskList = context.Tasks.Include("Users").Where(t => t.Users.Any(u => u.UserId == user.UserId)).ToList();
                //if (taskList.Count == 0)
                //    existingProject.Users.Remove(user);
                //else
                //    userList.Add(user);
                existingProject.ProjectOwners.Remove(owner);
            }
            foreach (User c in addedOwners)
            {

                if (context.Entry(c).State == System.Data.Entity.EntityState.Detached)
                    context.Users.Attach(c);
                existingProject.ProjectOwners.Add(c);
            }
            return ownerList;
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


        public List<Project> GetAssignedProjectByUser(User user)
        {
            List<Project> projectList = context.Projects.Where(p => p.IsActive).ToList();
            List<Project> projectListNew = new List<Project>();
            foreach (Project project in projectList)
            {
                if (project.Users.Exists(p => { return p.UserId == user.UserId; }))
                {
                    projectListNew.Add(project);
                }
            }
            return projectListNew;
        }

        public  List<Project> GetListbyName(string searchParam,params Expression<Func<Project, object>>[] includeProperties)
        {
            IQueryable<Project> query = context.Projects.Where(p => p.Name.ToLower().Contains(searchParam));
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query.ToList();
        }
    }

    public interface IProjectRepository : IDisposable
    {
        IQueryable<Project> All { get; }
        IQueryable<Project> AllIncluding(params Expression<Func<Project, object>>[] includeProperties);
        IQueryable<Project> AllbyUserIncluding(Guid userID, params Expression<Func<Project, object>>[] includeProperties);
        Project Find(long id);
        List<User> InsertOrUpdate(Project project);
        void Delete(long id);
        void Save();
        List<Project> GetAssignedProjectByUser(User user);
        List<Project> GetListbyName(string searchParam, params Expression<Func<Project, object>>[] includeProperties);
        Project FindIncludingProjectStatus(long projectID);
    }
}