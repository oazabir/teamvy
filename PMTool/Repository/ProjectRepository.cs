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

        public IQueryable<Project> AllbyUserIncluding(long userID,params Expression<Func<Project, object>>[] includeProperties)
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
        public List<Project> AllbyOwnerIncluding(long ownerId, params Expression<Func<Project, object>>[] includeProperties)
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
             
              Project project = context.Projects.Include("Users").Where(p=>p.ProjectID==id).FirstOrDefault();
              project.Sprints = project.Sprints.OrderByDescending(p => p.SprintID).ToList(); //To show sprint descending order added by Mahedee @06-03-14
              return project;
        }


        public List<UserProfile> InsertOrUpdate(Project project)
        {
            List<UserProfile> userList = new List<UserProfile>();
            List<UserProfile> ownerList = new List<UserProfile>();

            if (project.ProjectID == default(long)) {
                // New entity
                context.Projects.Add(project);
            } else {
                // Existing entity
                var existingProject = context.Projects.Include("Users")
                    .Where(t => t.ProjectID == project.ProjectID).FirstOrDefault<Project>();

                userList = UpdateAssignedUsers(project, existingProject);
                ownerList = UpdateProjectOwners(project, existingProject);
                //List<ProjectStatus> projectStatusList =UpdateProjectStatuses(project, existingProject);

                context.Entry(existingProject).CurrentValues.SetValues(project);
            }
            return userList;
        }

        //private List<ProjectStatus> UpdateProjectStatuses(Project project, Project existingProject)
        //{
        //    List<ProjectStatus> statusList = new List<ProjectStatus>();
        //    var deletedStatus = existingProject.ProjectStatuses.ToList<ProjectStatus>();
        //    var addedStatus = project.ProjectStatuses.ToList<ProjectStatus>();

        //    foreach (ProjectStatus item in existingProject.ProjectStatuses.ToList())
        //    {
        //        List<Task> taskList = context.Tasks.Where(t => t.ProjectStatus == item).ToList();
        //        if (taskList.Count == 0)
        //            existingProject.ProjectStatuses.Remove(item);
        //        else
        //            statusList.Add(item);
        //    }
        //    foreach (ProjectStatus c in addedStatus)
        //    {

        //        if (context.Entry(c).State == System.Data.Entity.EntityState.Detached)
        //            context.ProjectStatuses.Attach(c);
        //        existingProject.ProjectStatuses.Add(c);
        //    }
        //    return statusList;
        //}

        private List<UserProfile> UpdateAssignedUsers(Project project, Project existingProject)
        {
            List<UserProfile> userList = new List<UserProfile>();
            var deletedUsers = existingProject.Users.ToList<UserProfile>();
            var addedUsers = project.Users.ToList<UserProfile>();
           // deletedUsers.ForEach(c => existingProject.Users.Remove(c));
            foreach (UserProfile user in existingProject.Users.ToList())
            {
                List<Task> taskList = context.Tasks.Include("Users").Where(t => t.Users.Any(u => u.UserId == user.UserId)).ToList();
               if (taskList.Count == 0)
                   existingProject.Users.Remove(user);
               else
                   userList.Add(user);
            }
            foreach (UserProfile c in addedUsers)
            {

                if (context.Entry(c).State == System.Data.Entity.EntityState.Detached)
                    context.UserProfiles.Attach(c);
                existingProject.Users.Add(c);
            }
            return userList;
        }


        private List<UserProfile> UpdateProjectOwners(Project project, Project existingProject)
        {
            List<UserProfile> ownerList = new List<UserProfile>();
            var deletedOwners = existingProject.ProjectOwners.ToList<UserProfile>();
            var addedOwners = project.ProjectOwners.ToList<UserProfile>();
            // deletedUsers.ForEach(c => existingProject.Users.Remove(c));
            foreach (UserProfile owner in existingProject.ProjectOwners.ToList())
            {
                //List<Task> taskList = context.Tasks.Include("Users").Where(t => t.Users.Any(u => u.UserId == user.UserId)).ToList();
                //if (taskList.Count == 0)
                //    existingProject.Users.Remove(user);
                //else
                //    userList.Add(user);
                existingProject.ProjectOwners.Remove(owner);
            }
            foreach (UserProfile c in addedOwners)
            {

                if (context.Entry(c).State == System.Data.Entity.EntityState.Detached)
                    context.UserProfiles.Attach(c);
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


        public List<Project> GetAssignedProjectByUser(UserProfile user)
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

        public Project FindincludingSprint(long ProjectID)
        {
            Project project = context.Projects.Where(p => p.ProjectID == ProjectID).Include("Sprints").FirstOrDefault();
           
            //For showing sprint in descending order in kanban board. update by mahedee @ 06-03-14
            project.Sprints = project.Sprints.OrderByDescending(p => p.SprintID).ToList(); 
            return project;
        }


        internal Project FindAllDependancyOfProject(long ProjectID)
        {
            Project project = context.Projects.Where(p => p.ProjectID == ProjectID).Include("Users").Include("ProjectOwners").Include("ProjectStatuses").Include("Tasks").Include("Sprints").FirstOrDefault();
            return project;
        }


        public  List<Project> GetAll()
        {
            return context.Projects.ToList();
        }
    
    }

    public interface IProjectRepository : IDisposable
    {
        IQueryable<Project> All { get; }
        IQueryable<Project> AllIncluding(params Expression<Func<Project, object>>[] includeProperties);
        IQueryable<Project> AllbyUserIncluding(long userID, params Expression<Func<Project, object>>[] includeProperties);
        Project Find(long id);
        List<UserProfile> InsertOrUpdate(Project project);
        void Delete(long id);
        void Save();
        List<Project> GetAssignedProjectByUser(UserProfile user);
        List<Project> GetListbyName(string searchParam, params Expression<Func<Project, object>>[] includeProperties);
        Project FindIncludingProjectStatus(long projectID);
    }
}