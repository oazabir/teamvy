using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PMTool.Models;
using PMTool.Repository;
using System.Data.Entity;
using WebMatrix.WebData;

namespace PMTool.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";
            //Session.Abandon();
            //Session.Clear();

            UserProfile user = unitOfWork.UserRepository.GetUserByUserID((int)Membership.GetUser(WebSecurity.CurrentUserName).ProviderUserKey);
            List<Task> userTaskList = new List<Task>();
            List<Task> dueTaskList = new List<Task>();
            List<Task> overdueTaskList = new List<Task>();
            List<Task> todaysTaskList = new List<Task>();
            List<Task> dueTomorrowTaskList = new List<Task>();
            List<Task> futureTaskList = new List<Task>();

            userTaskList = unitOfWork.TaskRepository.GetTasksByUser(user).Where(p => p.ProjectStatus.Name.ToLower() != "closed").ToList();

            /*Task which is not closed will display to the dashboard. Updated by Mahedee @ 26-02-14*/
            overdueTaskList = userTaskList.Where(p => (p.EndDate < DateTime.Today) && (p.ProjectStatusID != null && p.ProjectStatus.Name != "Closed")).ToList();
            dueTaskList = userTaskList.Where(p => (p.StartDate < DateTime.Today && p.EndDate >= DateTime.Today) && (p.ProjectStatusID != null && p.ProjectStatus.Name != "Closed")).ToList();
            todaysTaskList = userTaskList.Where(p => p.StartDate == DateTime.Today && (p.ProjectStatusID != null && p.ProjectStatus.Name != "Closed")).ToList();
            dueTomorrowTaskList = userTaskList.Where(p => p.EndDate == DateTime.Today.AddDays(1) && (p.ProjectStatusID != null && p.ProjectStatus.Name != "Closed")).ToList();

            futureTaskList = userTaskList.Where(p => p.StartDate > DateTime.Today && (p.ProjectStatusID == null || p.ProjectStatus.Name != "Closed")).ToList();

            ViewBag.OverdueTask = overdueTaskList;
            ViewBag.DueTask = dueTaskList;
            ViewBag.TodaysTask = todaysTaskList;
            ViewBag.DueTomorrowTask = dueTomorrowTaskList;
            ViewBag.FutureTask = futureTaskList;

            return View(userTaskList);
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
            //UnitOfWork unitOfWork = new UnitOfWork();
            //User user = unitOfWork.UserRepository.GetUserByUserID((int)Membership.GetUser(WebSecurity.CurrentUserName).ProviderUserKey);
            //List<Notification> notificationList = unitOfWork.NotificationRepository.UserUnreadNotification(user);
            List<Notification> notificationList = new List<Notification>();
            return PartialView(notificationList);
        }

        public PartialViewResult _NotificationReadAll(List<Notification> notifications)
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            UserProfile user = unitOfWork.UserRepository.GetUserByUserID((int)Membership.GetUser(WebSecurity.CurrentUserName).ProviderUserKey);
            List<Notification> notificationList = unitOfWork.NotificationRepository.UserUnreadNotification(user);
            List<Notification> notificationListNew = new List<Notification>();
            foreach (Notification item in notificationList)
            {
                item.IsNoticed = true;
                unitOfWork.NotificationRepository.InsertOrUpdate(item);
            }
            unitOfWork.Save();
            return PartialView("_Notification", notificationListNew);
        }


        public PartialViewResult _TaskList(long projectID, long? statusID)
        {
            //List<Task> taskList;
            //Project project;
            //if (statusID != null && statusID != 0)
            //    GetTaskList(projectID, out taskList, out project, statusID);
            //else
            //    GetTaskList(projectID, out taskList, out project);


            //ViewBag.TaskStatus = GetAllStatus(projectID);
            //ViewBag.CurrentProject = project;
            //return PartialView(taskList);

            UserProfile user = unitOfWork.UserRepository.GetUserByUserID((int)Membership.GetUser(WebSecurity.CurrentUserName).ProviderUserKey);
            List<Task> userTaskList = new List<Task>();
            //List<Task> dueTaskList = new List<Task>();
            //List<Task> overdueTaskList = new List<Task>();
            //List<Task> todaysTaskList = new List<Task>();
            //List<Task> dueTomorrowTaskList = new List<Task>();
            //List<Task> futureTaskList = new List<Task>();

            userTaskList = unitOfWork.TaskRepository.GetTasksByUser(user).Where(p => p.ProjectStatus.Name.ToLower() != "closed").ToList();

            return PartialView(userTaskList);
        }

        private void GetTaskList(long projectID, out List<Task> taskList, out Project project, long? statusId)
        {
            taskList = new List<Task>();
            UserProfile user = unitOfWork.UserRepository.GetUserByUserID((int)Membership.GetUser(WebSecurity.CurrentUserName).ProviderUserKey);
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
            UserProfile user = unitOfWork.UserRepository.GetUserByUserID((int)Membership.GetUser(WebSecurity.CurrentUserName).ProviderUserKey);
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

        public ActionResult ChangeStatusView(long taskID, long? statusID)
        {
            Task task = unitOfWork.TaskRepository.Find(taskID);
            task.ProjectStatusID = statusID;
            unitOfWork.Save();
            //return RedirectToAction("ProjectTasks", new { projectID =task.ProjectID});
            List<Task> taskList = unitOfWork.TaskRepository.GetTasksByProjectID(task.ProjectID);
            //return PartialView("_TaskList", taskList.ToPagedList(1, defaultPageSize));
            return RedirectToAction("_TaskList", new { @projectID = task.ProjectID });
            //return RedirectToAction("Index");
        }

        public PartialViewResult AddTimeLog(long id)
        //public ViewResult AddTimeLog(long id)
        {
            Task task = unitOfWork.TaskRepository.Find(id);
            ViewBag.Task = task;
            ViewBag.TaskTimeLog = unitOfWork.TimeLogRepository.GetTimeLogByTaskId(id);
            TimeLog timeLog = new TimeLog();
            timeLog.SprintID = task.SprintID;
            timeLog.TaskID = id;
            //return View(timeLog);
            return PartialView(timeLog);
        }

        [HttpPost]
        //public PartialViewResult AddMessage(TaskMessage taskMessage)
        public ActionResult AddTimeLog(TimeLog timeLog)
        {
            if (timeLog != null)
            {
                timeLog.UserID = (int)Membership.GetUser(WebSecurity.CurrentUserName).ProviderUserKey;
                timeLog.CreatedBy = (int)Membership.GetUser(WebSecurity.CurrentUserName).ProviderUserKey;
                timeLog.ModifiedBy = (int)Membership.GetUser(WebSecurity.CurrentUserName).ProviderUserKey;
                timeLog.CreateDate = DateTime.Now;
                timeLog.ModificationDate = DateTime.Now;
                timeLog.ActionDate = DateTime.Now;

                unitOfWork.TimeLogRepository.InsertOrUpdate(timeLog);

                unitOfWork.Save();
            }
            return RedirectToAction("_TaskEntryLog", new { @taskId = timeLog.TaskID });
        }

        public PartialViewResult _TaskEntryLog(long taskId)
        {
            List<TimeLog> lstTimeLog = new List<TimeLog>();
            lstTimeLog = unitOfWork.TimeLogRepository.GetTimeLogByTaskId(taskId);
            return PartialView(lstTimeLog);
        }


    }
}
