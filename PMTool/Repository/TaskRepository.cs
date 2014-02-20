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

        public TaskRepository()
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
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public IQueryable<Task> ByProjectIncluding(long projectID, params Expression<Func<Task, object>>[] includeProperties)
        {
            IQueryable<Task> query = context.Tasks.Where(t => t.ProjectID == projectID && t.ParentTaskId == null);
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);

            }
            foreach (Task task in query)
            {
                task.ChildTask = context.Tasks.Where(t => t.ParentTaskId == task.TaskID).ToList();
                task.CreatedByUser = context.Users.Where(t => t.UserId == task.CreatedBy).FirstOrDefault();
                task.ProjectStatus = context.ProjectStatuses.Where(t => t.ProjectStatusID == task.ProjectStatusID).FirstOrDefault();
            }
            return query;
        }
        
        public List<Task> GetTasksBySprintID(long sprintID)
        {
            List<Task> taskList = context.Tasks.Where(t => t.SprintID == sprintID).ToList();
            foreach (Task task in taskList)
            {
                task.ChildTask = context.Tasks.Where(t => t.ParentTaskId == task.TaskID).ToList();
                task.CreatedByUser = context.Users.Where(t => t.UserId == task.CreatedBy).FirstOrDefault();
                task.ProjectStatus = context.ProjectStatuses.Where(t => t.ProjectStatusID == task.ProjectStatusID).FirstOrDefault();
            }
            return taskList;
        }

        public IQueryable<Task> ByProjectAndStatusIncluding(long projectID,long status, params Expression<Func<Task, object>>[] includeProperties)
        {
            IQueryable<Task> query = context.Tasks.Where(t => t.ProjectID == projectID && t.ParentTaskId == null && t.ProjectStatusID==status);
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);

            }
            foreach (Task task in query)
            {
                task.ChildTask = context.Tasks.Where(t => t.ParentTaskId == task.TaskID).ToList();
            }
            return query;
        }

        public IQueryable<Task> ByProjectIncluding(long projectID,User user,params Expression<Func<Task, object>>[] includeProperties)
        {
            IQueryable<Task> query = context.Tasks.Where(t => t.ProjectID == projectID && t.ParentTaskId == null && t.Users.Any(U=>U.UserId==user.UserId));
            
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            foreach (Task task in query.ToList())
            {
                task.ChildTask = context.Tasks.Where(t => t.ParentTaskId == task.TaskID).ToList();
            }
            return query;
        }

        

        public IQueryable<Task> AllSubTaskByProjectIncluding(long projectID,long TaskID, params Expression<Func<Task, object>>[] includeProperties)
        {
            IQueryable<Task> query = context.Tasks.Where(t => t.ProjectID == projectID && t.ParentTaskId != null && t.ParentTaskId==TaskID);
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);

            }
            foreach (Task task in query)
            {
                task.ParentTask = context.Tasks.Where(t => t.TaskID == task.ParentTaskId).FirstOrDefault();
            }
            return query;
        }

        public Task Find(long id)
        {
            Task objTask = new Task();
            //return 
            objTask = context.Tasks.Where(t => t.TaskID == id).Include(task => task.Users).Include(task => task.Followers).Include(task => task.CreatedByUser).FirstOrDefault();
            objTask.CreatedByUser = context.Users.Where(t => t.UserId == objTask.CreatedBy).FirstOrDefault();
            objTask.ProjectStatus = context.ProjectStatuses.Where(t => t.ProjectStatusID == objTask.ProjectStatusID).FirstOrDefault();

            //Task tasks = new Task();
            //tasks = context.Tasks.Where(t => t.TaskID == id).Include(task => tasks.Users).Include(task => task.Followers).Include(task => task.CreatedByUser).FirstOrDefault();
            //return tasks;
            //return context.Tasks.Where(t => t.TaskID == id).Include(task => task.Users).Include(task => task.Followers).FirstOrDefault()
            //    .CreatedByUser = context.Users.Where(t => t.UserId = ;

            return objTask;
        }

        public bool InsertOrUpdate(Task task)
        {
           bool isStatusChanged = false;
            if (task.TaskID == default(long))
            {
                // New entity
                context.Tasks.Add(task);

            }
            else
            {
                // Existing entity
                var existingTask = context.Tasks.Include("Users")
                   .Where(t => t.TaskID == task.TaskID).FirstOrDefault<Task>();

                if (task.Status != existingTask.Status)
                {
                    isStatusChanged = true;
                }

                UpdateAssignedUsers(task, existingTask);

                UpdateFollowers(task, existingTask);

                UpdateLabels(task, existingTask);
                context.Entry(existingTask).CurrentValues.SetValues(task);

            }
            return isStatusChanged;
        }

        private void UpdateLabels(Task task, Task existingTask)
        {
            var deletedLabels = existingTask.Labels.ToList<Label>();
            var addedLabels = task.Labels.ToList<Label>();
            deletedLabels.ForEach(c => existingTask.Labels.Remove(c));
            foreach (Label c in addedLabels)
            {

                if (context.Entry(c).State == System.Data.Entity.EntityState.Detached)
                    context.Labels.Attach(c);
                existingTask.Labels.Add(c);
            }
        }

        private void UpdateFollowers(Task task, Task existingTask)
        {
            var deletedFollowers = existingTask.Followers.ToList<User>();
            var addedFollowers = task.Followers.ToList<User>();
            deletedFollowers.ForEach(c => existingTask.Followers.Remove(c));
            foreach (User c in addedFollowers)
            {

                if (context.Entry(c).State == System.Data.Entity.EntityState.Detached)
                    context.Users.Attach(c);
                existingTask.Followers.Add(c);
            }
        }

        private void UpdateAssignedUsers(Task task, Task existingTask)
        {
            var deletedUsers = existingTask.Users.ToList<User>();
            var addedUsers = task.Users.ToList<User>();
            deletedUsers.ForEach(c => existingTask.Users.Remove(c));
            foreach (User c in addedUsers)
            {

                if (context.Entry(c).State == System.Data.Entity.EntityState.Detached)
                    context.Users.Attach(c);
                existingTask.Users.Add(c);
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

        public List<Task> GetTasksByProjectID(long projectID)
        {
            return context.Tasks.Where(t => t.ProjectID == projectID).ToList();
        }
    }

    public interface ITaskRepository : IDisposable
    {
        IQueryable<Task> All { get; }
        IQueryable<Task> AllIncluding(params Expression<Func<Task, object>>[] includeProperties);
        IQueryable<Task> ByProjectAndStatusIncluding(long projectID, long status, params Expression<Func<Task, object>>[] includeProperties);
        Task Find(long id);
        bool InsertOrUpdate(Task task);
        void Delete(long id);
        void Save();
        List<Task> GetTasksBySprintID(long sprintID);
        List<Task> GetTasksByProjectID(long projectID);
    }
}