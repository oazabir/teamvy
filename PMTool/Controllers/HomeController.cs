using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PMTool.Models;
using PMTool.Repository;
using System.Data.Entity;

namespace PMTool.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            User user = unitOfWork.UserRepository.GetUserByUserID((Guid)Membership.GetUser(WebSecurity.User.Identity.Name).ProviderUserKey);
            List<Task> userTaskList = new List<Task>();
            List<Task> overdueTaskList = new List<Task>();
            List<Task> dueTaskList = new List<Task>();
            List<Task> todaysTaskList = new List<Task>();
            List<Task> dueTomorrowTaskList = new List<Task>();
            List<Task> futureTaskList = new List<Task>();

            userTaskList = unitOfWork.TaskRepository.GetTasksByUser(user).ToList();

            //overdueTaskList = userTaskList.Where(p => (p.EndDate < DateTime.Today) && ((p.ProjectStatus.ProjectStatusID == null ? " " : p.ProjectStatus.Name) != "Close")).ToList();
            overdueTaskList = userTaskList.Where(p => (p.EndDate < DateTime.Today) && (p.ProjectStatus.Name != "Close")).ToList();
            dueTaskList = userTaskList.Where(p => (p.StartDate < DateTime.Today && p.EndDate >= DateTime.Today) && (p.ProjectStatus.Name != "Close")).ToList();
            todaysTaskList = userTaskList.Where(p => p.StartDate == DateTime.Today && p.ProjectStatus.Name != "Close").ToList();
            dueTomorrowTaskList = userTaskList.Where(p => p.EndDate == DateTime.Today.AddDays(1) && p.ProjectStatus.Name != "Close").ToList();

            futureTaskList = userTaskList.Where(p => p.StartDate > DateTime.Today && p.ProjectStatus.Name != "Close").ToList();

            ViewBag.OverdueTask = overdueTaskList;
            ViewBag.DueTask = dueTaskList;
            ViewBag.TodaysTask = todaysTaskList;
            ViewBag.DueTomorrowTask = dueTomorrowTaskList;
            ViewBag.FutureTask = futureTaskList;

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public PartialViewResult _Notification()
        {
            UnitOfWork unitOfWork = new UnitOfWork();
             User user = unitOfWork.UserRepository.GetUserByUserID((Guid)Membership.GetUser(WebSecurity.User.Identity.Name).ProviderUserKey);
            List<Notification> notificationList = unitOfWork.NotificationRepository.UserUnreadNotification(user);
            //ViewBag.Notifications = notificationList;
            return PartialView(notificationList);
        }
        
        public PartialViewResult _NotificationReadAll(List<Notification> notifications)
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            User user = unitOfWork.UserRepository.GetUserByUserID((Guid)Membership.GetUser(WebSecurity.User.Identity.Name).ProviderUserKey);
            List<Notification> notificationList = unitOfWork.NotificationRepository.UserUnreadNotification(user);
            List<Notification> notificationListNew = new List<Notification>();
            foreach (Notification item in notificationList)
            {
                item.IsNoticed = true;
                unitOfWork.NotificationRepository.InsertOrUpdate(item);
                unitOfWork.NotificationRepository.Delete(item.NotificationID);
            }
            unitOfWork.Save();
            return PartialView("_Notification", notificationListNew);
        }


        public PartialViewResult _TaskList(long projectID, long? statusID)
        {
            List<Task> taskList;
            Project project;
            if (statusID != null && statusID != 0)
                GetTaskList(projectID, out taskList, out project, statusID);
            else
                GetTaskList(projectID, out taskList, out project);


            //List<SelectListItem> statusList = new List<SelectListItem>();
            //statusList = unitOfWork.ProjectRepository.FindIncludingProjectStatus(projectID).ProjectStatuses.ToList();
            ViewBag.TaskStatus = GetAllStatus(projectID);

            //task.Project.ProjectStatuses;

            ViewBag.CurrentProject = project;
            //return PartialViewResult(taskList);
            //return View(taskList);
            return PartialView(taskList);
        }

        private void GetTaskList(long projectID, out List<Task> taskList, out Project project, long? statusId)
        {
            taskList = new List<Task>();
            User user = unitOfWork.UserRepository.GetUserByUserID((Guid)Membership.GetUser(WebSecurity.User.Identity.Name).ProviderUserKey);
            project = unitOfWork.ProjectRepository.Find(projectID);
            //If this project is created by the current user. Then he can see all task.
            if (project.CreatedBy == user.UserId)
                taskList = unitOfWork.TaskRepository.ByProjectIncluding(projectID, task => task.Project).Include(task => task.Priority).Include(task => task.ChildTask).Include(task => task.Users).Include(task => task.Followers).Include(task => task.Labels).Where(p => p.ProjectStatus.ProjectStatusID == statusId).ToList();

            //If this project is owned by the current user. Then he can see all task.
            else if (project.ProjectOwners.Contains(user))
                taskList = unitOfWork.TaskRepository.ByProjectIncluding(projectID, task => task.Project).Include(task => task.Priority).Include(task => task.ChildTask).Include(task => task.Users).Include(task => task.Followers).Include(task => task.Labels).Where(p => p.ProjectStatus.ProjectStatusID == statusId).ToList();

            else
                taskList = unitOfWork.TaskRepository.ByProjectIncluding(projectID, user, task => task.Project).Include(task => task.Priority).Include(task => task.ChildTask).Include(task => task.Users).Include(task => task.Followers).Include(task => task.Labels).Where(p => p.ProjectStatus.ProjectStatusID == statusId).ToList();

        }


        private void GetTaskList(long projectID, out List<Task> taskList, out Project project)
        {
            taskList = new List<Task>();
            User user = unitOfWork.UserRepository.GetUserByUserID((Guid)Membership.GetUser(WebSecurity.User.Identity.Name).ProviderUserKey);
            project = unitOfWork.ProjectRepository.Find(projectID);
            //If this project is created by the current user. Then he can see all task.
            if (project.CreatedBy == user.UserId)
                taskList = unitOfWork.TaskRepository.ByProjectIncluding(projectID, task => task.Project).Include(task => task.Priority).Include(task => task.ChildTask).Include(task => task.Users).Include(task => task.Followers).Include(task => task.Labels).ToList();

            //If this project is owned by the current user. Then he can see all task.
            else if (project.ProjectOwners.Contains(user))
                taskList = unitOfWork.TaskRepository.ByProjectIncluding(projectID, task => task.Project).Include(task => task.Priority).Include(task => task.ChildTask).Include(task => task.Users).Include(task => task.Followers).Include(task => task.Labels).ToList();

            else
                taskList = unitOfWork.TaskRepository.ByProjectIncluding(projectID, user, task => task.Project).Include(task => task.Priority).Include(task => task.ChildTask).Include(task => task.Users).Include(task => task.Followers).Include(task => task.Labels).ToList();

        }

        private List<SelectListItem> GetAllStatus(long ProjectID)
        {
            List<SelectListItem> allStatus = new List<SelectListItem>();
            List<ProjectStatus> statusList = unitOfWork.ProjectRepository.FindIncludingProjectStatus(ProjectID).ProjectStatuses;
            foreach (ProjectStatus status in statusList)
            {

                SelectListItem item = new SelectListItem { Value = status.ProjectStatusID.ToString(), Text = status.Name };
                allStatus.Add(item);
            }

            return allStatus;
        }


    }
}
