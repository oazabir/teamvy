using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PMTool.Models;
using PMTool.Repository;
using MvcPaging;

namespace PMTool.Controllers
{
    [Authorize]
    public class TasksController : BaseController
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        private const int defaultPageSize = 10;
        //
        // GET: /Tasks/

        public ViewResult Index()
        {
            return View(unitOfWork.TaskRepository.AllIncluding(task => task.Project).Include(task => task.Priority).Include(task => task.ChildTask).Include(task => task.Users).Include(task => task.Followers).Include(task => task.Labels).ToList());
        }

        public ActionResult ChangeStatusView(long taskID, long? statusID)
        {
            Task task = unitOfWork.TaskRepository.Find(taskID);
            task.ProjectStatusID = statusID;
            unitOfWork.Save();
            //return RedirectToAction("ProjectTasks", new { projectID =task.ProjectID});
            List<Task> taskList = unitOfWork.TaskRepository.GetTasksByProjectID(task.ProjectID);
            return PartialView("_TaskList", taskList.ToPagedList(1, defaultPageSize));
        }


        //GET: /Tasks/ProjectTasks?ProjectID=5
        public ViewResult ProjectTasks(long projectID, string SelectedStatusID, string SelectedPriorityID, string SelectedUserID, string SelectedSprintID, int? page)
        {
            int currentPageIndex = page.HasValue ? page.Value : 1;
            IList<Task> taskList;
            UserProfile user = unitOfWork.UserRepository.GetUserByUserID((int)Membership.GetUser(WebSecurity.CurrentUserName).ProviderUserKey);
            Project project = unitOfWork.ProjectRepository.Find(projectID);

            ViewBag.TaskStatus = GetAllStatus(projectID);

            ViewBag.PossiblePriorities = GetallPriority();
            ViewBag.PossibleSprints = GetallSprints(projectID);
            ViewBag.PossibleUsers = GetAllUser(projectID);
            ViewBag.CurrentProject = project;
            ViewBag.PageTitle = "Task List";
            Search search = new Search();
            ViewData["CurrentProjectID"] = project.ProjectID;
            ViewData["SelectedStatusID"] = SelectedStatusID;
            ViewData["SelectedPriorityID"] = SelectedPriorityID;
            ViewData["SelectedUserID"] = SelectedUserID;
            ViewData["SelectedSprintID"] = SelectedSprintID;


            search.SelectedProjectID = project.ProjectID;

            if (!string.IsNullOrEmpty(SelectedStatusID))
                search.SelectedStatusID = Convert.ToInt64(SelectedStatusID);
            if (!string.IsNullOrEmpty(SelectedPriorityID))
                search.SelectedPriorityID = Convert.ToInt64(SelectedPriorityID);

            if (!string.IsNullOrEmpty(SelectedUserID))
                search.SelectedUserID = Convert.ToInt64(SelectedUserID);

            if (!string.IsNullOrEmpty(SelectedSprintID))
                search.SelectedSprintID = Convert.ToInt64(SelectedSprintID);

            taskList = GetTasks(search);
            ViewBag.SearchObj = search;
            return View(taskList.ToPagedList(currentPageIndex, defaultPageSize));
        }


        #region Time Log
        //Action for your assigned task added by Mahedee @ 13-03-14
        public ViewResult MyAssignTask()
        {
            UserProfile user = unitOfWork.UserRepository.GetUserByUserID((int)Membership.GetUser(WebSecurity.CurrentUserName).ProviderUserKey);
            List<Task> myTaskList = unitOfWork.TaskRepository.GetTasksByUser(user).Where(task => task.ProjectStatus.Name.ToLower() != "closed").ToList();
            return View(myTaskList);
        }

        //Action for get all time log on corresponding task added by Mahedee @ 13-03-14
        public ViewResult TaskTimeLog(long taskId, long sprintId)
        {
            List<TimeLog> timeLog = new List<TimeLog>();
            timeLog = unitOfWork.TimeLogRepository.All.Where(p => p.TaskID == taskId).ToList();
            ViewBag.TaskId = taskId;
            ViewBag.SprintId = sprintId;
            return View(timeLog);
        }

        //Action for create time log on corresponding task added by Mahedee @ 13-03-14
        public ActionResult CreateTimeLog(long taskId, long sprintId)
        {
            //Task task = unitOfWork.TaskRepository.get
            TimeLog timelog = new TimeLog();
            timelog.TaskID = taskId;
            timelog.SprintID = sprintId;

            return View(timelog);
        }


        //Action to create time log on corresponding task added by Mahedee @ 18-03-14
        [HttpPost]
        public ActionResult CreateTimeLog(TimeLog timeLog)
        {
            timeLog.CreatedBy = (int)Membership.GetUser(WebSecurity.CurrentUserName).ProviderUserKey;
            timeLog.ModifiedBy = (int)Membership.GetUser(WebSecurity.CurrentUserName).ProviderUserKey;
            timeLog.UserID = (int)Membership.GetUser(WebSecurity.CurrentUserName).ProviderUserKey;
            timeLog.CreateDate = DateTime.Now;
            timeLog.ModificationDate = DateTime.Now;
            timeLog.ActionDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                unitOfWork.TimeLogRepository.InsertOrUpdate(timeLog);
                unitOfWork.Save();
                return RedirectToAction("TaskTimeLog", new { @taskId = timeLog.TaskID, @sprintId = timeLog.SprintID });
            }
            return RedirectToAction("TaskTimeLog", new { @id = timeLog.TaskID });
        }

        #endregion Time Log




        #region Sprint
        //Added for editing sprint from kanban board by Mahedee @ 11-03-14
        public PartialViewResult EditSprint(long sprintId)
        {
            Sprint sprint = unitOfWork.SprintRepository.Find(sprintId);
            return PartialView(sprint);
        }

        //Added for editing sprint from kanban board by Mahedee @ 11-03-14
        [HttpPost]
        public PartialViewResult EditSprint(Sprint sprint)
        {
            unitOfWork.SprintRepository.InsertOrUpdate(sprint);
            unitOfWork.Save();
            List<Task> taskList = unitOfWork.TaskRepository.ByProjectIncluding(sprint.ProjectID, task => task.Project).Include(task => task.Priority).Include(task => task.ChildTask).Include(task => task.Users).Include(task => task.Followers).Include(task => task.Labels).ToList();
            ViewBag.CurrentProject = unitOfWork.ProjectRepository.Find(sprint.ProjectID);
            return PartialView("_Kanban", taskList);
        }


        //Added for deleting sprint from kanban board by Mahedee @ 10-03-14
        public PartialViewResult DeleteSprint(long projectID, long sprintId)
        {

            string status = "";
            try
            {
                List<Task> tasklist = unitOfWork.TaskRepository.GetTasksBySprintID(sprintId);
                if (tasklist.Count == 0)
                {
                    unitOfWork.SprintRepository.Delete(sprintId);
                    unitOfWork.Save();
                    status = "success";
                }
                else
                {
                    status = "Error : One or more task is already attached in this sprint...";
                }
            }
            catch
            {
                status = "Error : One or more task is already attached in this sprint...";
            }
            //return Content(status);
            //return RedirectToAction("ProjectTasks", new { @ProjectID = task.ProjectID });

            ViewBag.CurrentProject = unitOfWork.ProjectRepository.Find(projectID);
            List<Task> taskList = unitOfWork.TaskRepository.ByProjectIncluding(projectID, t => t.Project).Include(t => t.Priority).Include(t => t.ChildTask).Include(t => t.Users).Include(t => t.Followers).Include(t => t.Labels).ToList();
            return PartialView("_Kanban", taskList);

        }

        #endregion sprint

        public PartialViewResult _TaskList(long projectID, long? statusID, int? page, string SelectedStatusID, string SelectedPriorityID, string SelectedUserID, string SelectedSprintID)
        {
            int currentPageIndex = page.HasValue ? page.Value : 1;
            IList<Task> taskList;
            Project project;
            if (statusID != null && statusID != 0)
                GetTaskList(projectID, out taskList, out project, statusID);
            else
                GetTaskList(projectID, out taskList, out project);

            ViewBag.TaskStatus = GetAllStatus(projectID);
            ViewBag.CurrentProject = project;
            taskList = taskList.ToPagedList(currentPageIndex, defaultPageSize);
            return PartialView(taskList);
        }

        private void GetTaskList(long projectID, out IList<Task> taskList, out Project project, long? statusId)
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


        private void GetTaskList(long projectID, out IList<Task> taskList, out Project project)
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




        //
        // GET: /Tasks/SubTaskList?taskID=5
        public ViewResult SubTaskList(long projectID, long taskID)
        {
            List<Task> taskList = unitOfWork.TaskRepository.AllSubTaskByProjectIncluding(projectID, taskID, task => task.Project).Include(task => task.Priority).Include(task => task.ChildTask).Include(task => task.Users).Include(task => task.Followers).Include(task => task.Labels).ToList();
            ViewBag.CurrentProject = unitOfWork.ProjectRepository.Find(projectID);
            return View(taskList);
        }

        //
        // GET: /Tasks/Details/5

        public ViewResult Details(long id)
        {
            Task task = unitOfWork.TaskRepository.Find(id);
            return View(task);
        }


        public PartialViewResult TaskDetails(long id)
        {
            Task task = unitOfWork.TaskRepository.Find(id);
            return PartialView(task);
        }

        public PartialViewResult ShowEstimateView(long id)
        {
            Task task = unitOfWork.TaskRepository.Find(id);
            return PartialView(task);
        }

        [HttpPost]
        public ActionResult ShowEstimateView(Task task)
        {
            Task oldtask = unitOfWork.TaskRepository.Find(task.TaskID);
            oldtask.TaskHour = task.TaskHour;
            unitOfWork.TaskRepository.InsertOrUpdate(oldtask);
            unitOfWork.Save();
            List<Task> tasklist = unitOfWork.TaskRepository.GetTasksByProjectID(oldtask.ProjectID);
            return RedirectToAction("_TaskList", new { projectID = oldtask.ProjectID });
            //return RedirectToAction("ProjectTasks", new { projectID = oldtask.ProjectID });
        }


        //
        // GET: /Tasks/Create?ProjectID=5

        public ActionResult Create(long ProjectID)
        {
            List<SelectListItem> allUsers = GetAllUser(ProjectID);
            ViewBag.PossibleUsers = allUsers;

            ViewBag.PossibleProjects = unitOfWork.ProjectRepository.All;
            ViewBag.PossiblePriorities = unitOfWork.PriorityRepository.All;
            ViewBag.PossibleSprints = unitOfWork.SprintRepository.AllByProjectID(ProjectID);
            List<SelectListItem> allLabels = GetAllLabel();
            ViewBag.PossibleLabels = allLabels;
            Task task = new Task();
            task.ProjectID = ProjectID;
            //task.StartDate = DateTime.Now;
            //task.EndDate = DateTime.Now;

            GetAllStatus(task);

            return View(task);
        }

        private void GetAllStatus(Task task)
        {
            List<SelectListItem> statusList = new List<SelectListItem>();
            task.Project = unitOfWork.ProjectRepository.FindIncludingProjectStatus(task.ProjectID);
            if (task.Project.ProjectStatuses != null)
            {
                foreach (ProjectStatus item in task.Project.ProjectStatuses)
                {
                    SelectListItem listitem = new SelectListItem();
                    listitem.Value = item.ProjectStatusID.ToString();
                    listitem.Text = item.Name;
                    if (item.ProjectStatusID == task.ProjectStatusID)
                    {
                        listitem.Selected = true;
                    }
                    else
                    {
                        listitem.Selected = false;
                    }
                    statusList.Add(listitem);
                }
            }
            ViewBag.PossibleTaskStatus = task.Project.ProjectStatuses;
        }

        //
        // GET: /Tasks/CreateSubTask?ProjectID=5&TaskID=1

        public ActionResult CreateSubTask(long ProjectID, long TaskID)
        {
            List<SelectListItem> allUsers = GetAllUser(ProjectID);
            ViewBag.PossibleUsers = allUsers;
            ViewBag.PossibleProjects = unitOfWork.ProjectRepository.All;
            ViewBag.PossiblePriorities = unitOfWork.PriorityRepository.All;
            ViewBag.PossibleSprints = unitOfWork.SprintRepository.AllByProjectID(ProjectID);

            List<SelectListItem> allLabels = GetAllLabel();
            ViewBag.PossibleLabels = allLabels;
            Task task = new Task();
            task.ProjectID = ProjectID;
            task.StartDate = DateTime.Now;
            task.EndDate = DateTime.Now;
            task.ParentTaskId = TaskID;
            task.ParentTask = unitOfWork.TaskRepository.Find(TaskID);
            GetAllStatus(task);
            return View(task);
        }

        //
        // POST: /Tasks/Create

        [HttpPost]
        public ActionResult Create(Task task)
        {
            task.CreatedBy = (int)Membership.GetUser().ProviderUserKey;
            task.ModifiedBy = (int)Membership.GetUser().ProviderUserKey;
            task.CreateDate = DateTime.Now;
            task.ModificationDate = DateTime.Now;
            task.ActionDate = DateTime.Now;
            task.IsActive = true;
            if (ModelState.IsValid)
            {
                TaskPropertyChange change = unitOfWork.TaskRepository.InsertOrUpdate(task);
                AddAssignUser(task);
                AddFollower(task);
                AddLabel(task);
                unitOfWork.Save();
                SaveNotification(task, true, false, change);
                return RedirectToAction("ProjectTasks", new { @ProjectID = task.ProjectID });
            }
            List<SelectListItem> allUsers = GetAllUser(task.ProjectID);
            ViewBag.PossibleUsers = allUsers;
            ViewBag.PossibleProjects = unitOfWork.ProjectRepository.All;
            ViewBag.PossiblePriorities = unitOfWork.PriorityRepository.All;
            ViewBag.PossibleSprints = unitOfWork.SprintRepository.AllByProjectID(task.ProjectID);

            List<SelectListItem> allLabels = GetAllLabel();
            ViewBag.PossibleLabels = allLabels;
            GetAllStatus(task);
            return View(task); //return RedirectToAction("ProjectTasks", new { @ProjectID = task.ProjectID });
        }

        private void SaveNotification(Task task, bool isTaskInsert, bool isSubTask, TaskPropertyChange change)
        {
            #region Old
            //string phrase = "";
            //if (task.Users.Count >0)
            //{
            //    foreach (User user in task.Users)
            //    {

            //        Notification notification = new Notification();
            //        if (isTaskInsert)
            //        {
            //            if (!isSubTask)
            //            {
            //                phrase = " Has assigned " + user.FirstName + " " + user.LastName + " on the Task --";
            //            }
            //            else
            //            {
            //                phrase = " Has assigned  " + user.FirstName + " " + user.LastName + "  on the Subtask --";
            //            }

            //            User createdUser = unitOfWork.UserRepository.GetUserByUserID(task.CreatedBy);
            //            notification.Title = createdUser.FirstName + " " + createdUser.LastName + phrase +" "+ task.Title;


            //        }
            //        else
            //        {
            //            User modifiedUser = unitOfWork.UserRepository.GetUserByUserID((int)Membership.GetUser(WebSecurity.CurrentUserName).ProviderUserKey);
            //            phrase = phrase + modifiedUser.FirstName + " " + modifiedUser.LastName + " ";
            //            if (!change.IsSatausChanged)
            //            {
            //                if (!isSubTask)
            //                {
            //                    phrase = phrase + " Has modify the Task --";
            //                }
            //                else
            //                {
            //                    phrase = phrase + " Has modify the Subtask --";
            //                }
            //            }
            //            else
            //            {
            //                string status = "";
            //                if (!isSubTask)
            //                {
            //                    if (task.ProjectStatusID != null)
            //                    {
            //                        ProjectStatus col = unitOfWork.ProjectStatusRepository.Find(Convert.ToInt64(task.ProjectStatusID));
            //                        status = col.Name;
            //                    }
            //                    phrase = phrase + string.Format(" Has changed the task status to {0} --", status);
            //                }
            //                else
            //                {
            //                    phrase = phrase + string.Format(" Has changed the subtask status to {0} --", status);
            //                }
            //            }

            //            if (!change.IsStartDateChanged)
            //            {
            //                string date = "";
            //                if (task.StartDate != null)
            //                {
            //                    date = task.StartDate.Value.ToShortDateString();
            //                }
            //                else
            //                {
            //                    date = "not defined";
            //                }
            //                if (!isSubTask)
            //                {

            //                    phrase = phrase + string.Format(" Has changed the task Start Date to {0} --", date);
            //                }
            //                else
            //                {
            //                    phrase = phrase + string.Format(" Has changed the subtask Start Date to {0} --", date);
            //                }
            //            }

            //            if (!change.IsEndtDateChanged)
            //            {
            //                string date = "";
            //                if (task.EndDate != null)
            //                {
            //                    date = task.EndDate.Value.ToShortDateString();
            //                }
            //                else
            //                {
            //                    date = "not defined";
            //                }
            //                if (!isSubTask)
            //                {

            //                    phrase = phrase + string.Format(" Has changed the task End Date to {0} --", date);
            //                }
            //                else
            //                {
            //                    phrase = phrase + string.Format(" Has changed the subtask End Date to {0} --", date);
            //                }
            //            }

            //            //User modifiedUser = unitOfWork.UserRepository.GetUserByUserID(task.ModifiedBy);
            //            notification.Title = phrase + task.Title;
            //        }
            //        notification.UserID = user.UserId;
            //        notification.Description = notification.Title;
            //        notification.ProjectID = task.ProjectID;
            //        notification.TaskID = task.TaskID;
            //        unitOfWork.NotificationRepository.InsertOrUpdate(notification);

            //        try
            //        {
            //            string logComment = phrase;
            //            logComment = logComment + "on " + DateTime.Now.ToShortDateString();
            //            TaskActivityLog log = unitOfWork.TaskActivityLogRepository.FindByTaskID(task.TaskID);
            //            if (log == null)
            //            {
            //                TaskActivityLog logNew = new TaskActivityLog();
            //                logNew.TaskID = task.TaskID;
            //                logNew.Comment = phrase;
            //                logNew.CreateDate = DateTime.Now;
            //                logNew.ModificationDate = DateTime.Now;
            //                unitOfWork.TaskActivityLogRepository.InsertOrUpdate(logNew);
            //                unitOfWork.Save();
            //            }
            //            else
            //            {
            //                log.Comment = log.Comment + " ; " + logComment;
            //                unitOfWork.Save();
            //            }
            //        }
            //        catch
            //        {
            //        }

            //        unitOfWork.Save();
            //    }
            //}
            //if (task.Followers.Count>0)
            //{
            //    foreach (User user in task.Followers)
            //    {
            //        Notification notification = new Notification();
            //        if (isTaskInsert)
            //        {
            //            if (!isSubTask)
            //            {
            //                phrase = " Has added  " + user.FirstName + " " + user.LastName + "  as a follower on the Task --";
            //            }
            //            else
            //            {
            //                phrase = " Has added  " + user.FirstName + " " + user.LastName + "  as a follower on the Subtask --";
            //            }
            //            User createdUser = unitOfWork.UserRepository.GetUserByUserID(task.CreatedBy);
            //            notification.Title = createdUser.FirstName + " " + createdUser.LastName + " " + phrase + task.Title;
            //        }
            //        else
            //        {
            //            if (!isSubTask)
            //            {
            //                phrase = " Has modify the Task --";
            //            }
            //            else
            //            {
            //                phrase = " Has modify the Subtask --";
            //            }
            //            User modifiedUser = unitOfWork.UserRepository.GetUserByUserID(task.ModifiedBy);
            //            notification.Title = modifiedUser.FirstName + " " + modifiedUser.LastName +" "+ phrase + task.Title;
            //        }

            //        if (!change.IsSatausChanged)
            //        {
            //            if (!isSubTask)
            //            {
            //                phrase = phrase + " Has modify the Task --";
            //            }
            //            else
            //            {
            //                phrase = phrase + " Has modify the Subtask --";
            //            }
            //        }
            //        else
            //        {
            //            string status = "";
            //            if (!isSubTask)
            //            {
            //                if (task.ProjectStatusID != null)
            //                {
            //                    ProjectStatus col = unitOfWork.ProjectStatusRepository.Find(Convert.ToInt64(task.ProjectStatusID));
            //                    status = col.Name;
            //                }
            //                phrase = phrase + string.Format(" Has changed the task status to {0} --", status);
            //            }
            //            else
            //            {
            //                phrase = phrase + string.Format(" Has changed the subtask status to {0} --", status);
            //            }
            //        }

            //        if (!change.IsStartDateChanged)
            //        {
            //            string date = "";
            //            if (task.StartDate != null)
            //            {
            //                date = task.StartDate.Value.ToShortDateString();
            //            }
            //            else
            //            {
            //                date = "not defined";
            //            }
            //            if (!isSubTask)
            //            {

            //                phrase = phrase + string.Format(" Has changed the task Start Date to {0} --", date);
            //            }
            //            else
            //            {
            //                phrase = phrase + string.Format(" Has changed the subtask Start Date to {0} --", date);
            //            }
            //        }

            //        if (!change.IsEndtDateChanged)
            //        {
            //            string date = "";
            //            if (task.EndDate != null)
            //            {
            //                date = task.EndDate.Value.ToShortDateString();
            //            }
            //            else
            //            {
            //                date = "not defined";
            //            }
            //            if (!isSubTask)
            //            {

            //                phrase = phrase + string.Format(" Has changed the task End Date to {0} --", date);
            //            }
            //            else
            //            {
            //                phrase = phrase + string.Format(" Has changed the subtask End Date to {0} --", date);
            //            }
            //        }

            //        notification.UserID = user.UserId;
            //        notification.Description = notification.Title;
            //        notification.ProjectID = task.ProjectID;
            //        notification.TaskID = task.TaskID;
            //        unitOfWork.NotificationRepository.InsertOrUpdate(notification);

            //        try
            //        {
            //            string logComment = phrase;
            //            logComment = logComment + "on " + DateTime.Now.ToShortDateString();
            //            TaskActivityLog log = unitOfWork.TaskActivityLogRepository.FindByTaskID(task.TaskID);
            //            if (log == null)
            //            {
            //                TaskActivityLog logNew = new TaskActivityLog();
            //                logNew.TaskID = task.TaskID;
            //                logNew.Comment = phrase;
            //                logNew.CreateDate = DateTime.Now;
            //                logNew.ModificationDate = DateTime.Now;
            //                unitOfWork.TaskActivityLogRepository.InsertOrUpdate(logNew);
            //                unitOfWork.Save();
            //            }
            //            else
            //            {
            //                log.Comment = log.Comment + " ; " + logComment;
            //                unitOfWork.Save();
            //            }
            //        }
            //        catch
            //        {
            //        }

            //        unitOfWork.Save();
            //    }
            //}
            //if (task.Users.Count == 0 && task.Followers.Count == 0)
            //{
            //    try
            //    {
            //        if (phrase != "")
            //        {
            //            User modifiedUser = unitOfWork.UserRepository.GetUserByUserID((int)Membership.GetUser(WebSecurity.CurrentUserName).ProviderUserKey);
            //            string logComment = phrase;
            //            logComment = logComment + "on " + DateTime.Now.ToShortDateString();
            //            TaskActivityLog log = unitOfWork.TaskActivityLogRepository.FindByTaskID(task.TaskID);
            //            if (log == null)
            //            {
            //                TaskActivityLog logNew = new TaskActivityLog();
            //                logNew.TaskID = task.TaskID;
            //                logNew.Comment = modifiedUser.FirstName + " " + modifiedUser.LastName + " Created the task " + task.Title + " on " + DateTime.Now.ToShortDateString();
            //                logNew.CreateDate = DateTime.Now;
            //                logNew.ModificationDate = DateTime.Now;
            //                unitOfWork.TaskActivityLogRepository.InsertOrUpdate(logNew);
            //                unitOfWork.Save();
            //            }
            //            else
            //            {
            //                log.Comment = log.Comment + " ; " + logComment;
            //                unitOfWork.Save();
            //            }
            //        }
            //    }
            //    catch
            //    {
            //    }
            //}
            #endregion

            # region if Insert
            string status = "";

            if (isTaskInsert)
            {
                UserProfile createdUser = unitOfWork.UserRepository.GetUserByUserID(task.CreatedBy);
                status = createdUser.FirstName + " " + createdUser.LastName + " has created a task";
                if (!isSubTask)
                {
                    if (task.Users.Count > 0)
                    {
                        foreach (UserProfile user in task.Users)
                        {
                            Notification notification = new Notification();
                            notification.Title = status + " for you";
                            notification.UserID = user.UserId;
                            notification.Description = notification.Title;
                            notification.ProjectID = task.ProjectID;
                            notification.TaskID = task.TaskID;
                            unitOfWork.NotificationRepository.InsertOrUpdate(notification);
                        }
                    }
                }
                else
                {
                    if (task.ParentTaskId != null)
                    {
                        Task parentTask = unitOfWork.TaskRepository.Find(Convert.ToInt64(task.ParentTaskId));
                        status = status + " has created a sub task under the task " + parentTask.Title;
                        if (task.Users.Count > 0)
                        {
                            foreach (UserProfile user in task.Users)
                            {
                                Notification notification = new Notification();
                                notification.Title = status + " for you";
                                notification.UserID = user.UserId;
                                notification.Description = notification.Title;
                                notification.ProjectID = task.ProjectID;
                                notification.TaskID = task.TaskID;
                                unitOfWork.NotificationRepository.InsertOrUpdate(notification);
                            }
                        }
                    }
                }
            }
            #endregion
            # region if Insert
            else
            {
                UserProfile modifieddUser = unitOfWork.UserRepository.GetUserByUserID(task.CreatedBy);
                status = modifieddUser.FirstName + " " + modifieddUser.LastName + " changed";
                status = GetModifiedStatus(status, change, task);
                if (!isSubTask)
                {
                    if (task.Users.Count > 0)
                    {
                        foreach (UserProfile user in task.Users)
                        {
                            Notification notification = new Notification();
                            notification.Title = status;
                            notification.UserID = user.UserId;
                            notification.Description = notification.Title;
                            notification.ProjectID = task.ProjectID;
                            notification.TaskID = task.TaskID;
                            unitOfWork.NotificationRepository.InsertOrUpdate(notification);
                        }
                    }
                }
            }

            try
            {
                string logComment = status;
                logComment = logComment + "on " + DateTime.Now.ToShortDateString();
                TaskActivityLog log = unitOfWork.TaskActivityLogRepository.FindByTaskID(task.TaskID);
                if (log == null)
                {
                    TaskActivityLog logNew = new TaskActivityLog();
                    logNew.TaskID = task.TaskID;
                    logNew.Comment = status;
                    logNew.CreateDate = DateTime.Now;
                    logNew.ModificationDate = DateTime.Now;
                    unitOfWork.TaskActivityLogRepository.InsertOrUpdate(logNew);
                    unitOfWork.Save();
                }
                else
                {
                    log.Comment = log.Comment + " ; " + logComment;
                    unitOfWork.Save();
                }
            }
            catch
            {
            }

            unitOfWork.Save();
            #endregion
        }

        private string GetModifiedStatus(string status, TaskPropertyChange change, Task task)
        {
            string phrase1 = "";
            string phrase2 = "";
            if (change.IsStartDateChanged)
            {
                status = status + " Start Date from ";
                phrase1 = change.FromSatrtDate != null ? change.FromSatrtDate.Value.ToShortDateString() : " undefined ";
                phrase2 = change.ToSatrtDate != null ? change.ToSatrtDate.Value.ToShortDateString() : " undefined ";
                status = status + " " + phrase1 + " to " + phrase2;
            }
            if (change.IsEndtDateChanged)
            {
                status = status + " End Date from ";
                phrase1 = change.FromEndDate != null ? change.FromEndDate.Value.ToShortDateString() : " undefined ";
                phrase2 = change.ToEndDate != null ? change.ToEndDate.Value.ToShortDateString() : " undefined ";
                status = status + " " + phrase1 + " to " + phrase2;
            }

            if (change.IsSatausChanged)
            {
                status = status + " Status from " + change.FromSataus + " to " + change.ToStatus;
            }

            return status;
        }

        //
        // POST: /Tasks/CreateSubTask

        [HttpPost]
        public ActionResult CreateSubTask(Task task)
        {
            task.TaskID = 0;
            task.CreatedBy = (int)Membership.GetUser().ProviderUserKey;
            task.ModifiedBy = (int)Membership.GetUser().ProviderUserKey;
            task.CreateDate = DateTime.Now;
            task.ModificationDate = DateTime.Now;
            task.ActionDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                TaskPropertyChange change = unitOfWork.TaskRepository.InsertOrUpdate(task);
                AddAssignUser(task);
                AddFollower(task);
                AddLabel(task);
                unitOfWork.Save();
                SaveNotification(task, true, true, change);
                return RedirectToAction("ProjectTasks", new { @ProjectID = task.ProjectID });
            }
            List<SelectListItem> allUsers = GetAllUser(task.ProjectID);
            ViewBag.PossibleUsers = allUsers;
            ViewBag.PossibleProjects = unitOfWork.ProjectRepository.All;
            ViewBag.PossiblePriorities = unitOfWork.PriorityRepository.All;
            ViewBag.PossibleSprints = unitOfWork.SprintRepository.AllByProjectID(task.ProjectID);

            List<SelectListItem> allLabels = GetAllLabel();
            ViewBag.PossibleLabels = allLabels;
            GetAllStatus(task);
            return View(task);
        }

        private void AddLabel(Task task)
        {
            task.Labels = new List<Label>();
            if (task.SelectedLabels != null)
            {
                foreach (string labelID in task.SelectedLabels)
                {
                    Label label = unitOfWork.LabelRepository.Find(Convert.ToInt64(labelID));
                    task.Labels.Add(label);
                }
            }
        }

        private void AddFollower(Task task)
        {
            task.Followers = new List<UserProfile>();
            if (task.SelectedFollowedUsers != null)
            {
                foreach (string userID in task.SelectedFollowedUsers)
                {
                    UserProfile user = unitOfWork.UserRepository.GetUserByUserID(Convert.ToInt64(userID));
                    task.Followers.Add(user);
                }
            }
        }

        private void AddAssignUser(Task task)
        {
            task.Users = new List<UserProfile>();
            if (task.SelectedAssignedUsers != null)
            {
                foreach (string userID in task.SelectedAssignedUsers)
                {
                    UserProfile user = unitOfWork.UserRepository.GetUserByUserID(Convert.ToInt64(userID));
                    task.Users.Add(user);

                }
            }
        }

        //
        // GET: /Tasks/Edit/5


        public ActionResult Edit(long id)
        {
            Task task = unitOfWork.TaskRepository.Find(id);
            ViewBag.PossibleProjects = unitOfWork.ProjectRepository.All;
            ViewBag.PossiblePriorities = unitOfWork.PriorityRepository.All;
            ViewBag.PossibleSprints = unitOfWork.SprintRepository.AllByProjectID(task.ProjectID);

            List<SelectListItem> allUsers = GetAllUser(task.ProjectID);
            ViewBag.PossibleUsers = allUsers;

            task.SelectedAssignedUsers = task.Users.Select(u => u.UserId.ToString()).ToList();
            task.SelectedFollowedUsers = task.Followers.Select(u => u.UserId.ToString()).ToList();
            task.SelectedLabels = task.Labels.Select(u => u.LabelID.ToString()).ToList();
            List<string> statusList = new List<string>();
            if (task.Status != null)
                statusList.Add(task.Status.ToString());
            List<SelectListItem> allLabels = GetAllLabel();
            ViewBag.PossibleLabels = allLabels;


            GetAllStatus(task);

            MakeNotificationReadonly();


            return View(task);
        }

        public ActionResult EditFromKanban(long id)
        {
            Task task = unitOfWork.TaskRepository.Find(id);
            ViewBag.PossibleProjects = unitOfWork.ProjectRepository.All;
            ViewBag.PossiblePriorities = unitOfWork.PriorityRepository.All;
            ViewBag.PossibleSprints = unitOfWork.SprintRepository.AllByProjectID(task.ProjectID);

            List<SelectListItem> allUsers = GetAllUser(task.ProjectID);
            ViewBag.PossibleUsers = allUsers;

            task.SelectedAssignedUsers = task.Users.Select(u => u.UserId.ToString()).ToList();
            task.SelectedFollowedUsers = task.Followers.Select(u => u.UserId.ToString()).ToList();
            task.SelectedLabels = task.Labels.Select(u => u.LabelID.ToString()).ToList();
            List<string> statusList = new List<string>();
            if (!string.IsNullOrEmpty(task.Status))
            {
                statusList.Add(task.Status.ToString());
            }
            List<SelectListItem> allLabels = GetAllLabel();
            ViewBag.PossibleLabels = allLabels;


            GetAllStatus(task);

            MakeNotificationReadonly();


            return PartialView(task);
        }

        [HttpPost]
        public PartialViewResult EditFromKanban(Task task)
        {
            task.ModifiedBy = (int)Membership.GetUser().ProviderUserKey;
            task.ModificationDate = DateTime.Now;
            task.ActionDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                AddAssignUser(task);
                AddFollower(task);
                AddLabel(task);

                TaskPropertyChange change = unitOfWork.TaskRepository.InsertOrUpdate(task);

                unitOfWork.Save();
                if (task.ParentTaskId != null)
                {
                    SaveNotification(task, false, true, change);
                }
                else
                {
                    SaveNotification(task, false, false, change);
                }
                List<SelectListItem> allUsers = GetAllUser(task.ProjectID);
                ViewBag.PossibleUsers = allUsers;
                ViewBag.PossibleProjects = unitOfWork.ProjectRepository.All;
                ViewBag.PossiblePriorities = unitOfWork.PriorityRepository.All;
                ViewBag.PossibleSprints = unitOfWork.SprintRepository.AllByProjectID(task.ProjectID);

                List<SelectListItem> allLabels = GetAllLabel();
                ViewBag.PossibleLabels = allLabels;
                GetAllStatus(task);

                Project project = unitOfWork.ProjectRepository.Find(task.ProjectID);
                ViewBag.CurrentProject = project;
                List<string> statusList = new List<string>();
                if (!string.IsNullOrEmpty(project.allStatus))
                {
                    statusList = project.allStatus.Split(',').ToList();
                }
                ViewBag.AllStatus = statusList;
                List<Task> taskList = new List<Task>();
                taskList = unitOfWork.TaskRepository.ByProjectIncluding(task.ProjectID, t => t.Project).Include(t => t.Priority).Include(t => t.ChildTask).Include(t => t.Users).Include(t => t.Followers).Include(t => t.Labels).ToList();

                // return RedirectToAction("Kanban", new { @ProjectID = task.ProjectID });
                return PartialView("_Kanban", taskList);
            }
            else
            {
                ModelState.AddModelError("Error", "Ex: This login failed");
            }
            return null;
        }


        public ActionResult CreateFromKanban(long id)
        {
            Task task = new Task();
            task.StartDate = DateTime.Now;
            task.EndDate = DateTime.Now;
            task.ProjectID = id;
            ViewBag.PossibleProjects = unitOfWork.ProjectRepository.All;
            ViewBag.PossiblePriorities = unitOfWork.PriorityRepository.All;
            ViewBag.PossibleSprints = unitOfWork.SprintRepository.AllByProjectID(task.ProjectID);

            List<SelectListItem> allUsers = GetAllUser(task.ProjectID);
            ViewBag.PossibleUsers = allUsers;

            //task.SelectedAssignedUsers = task.Users.Select(u => u.UserId.ToString()).ToList();
            //task.SelectedFollowedUsers = task.Followers.Select(u => u.UserId.ToString()).ToList();
            //task.SelectedLabels = task.Labels.Select(u => u.LabelID.ToString()).ToList();
            List<string> statusList = new List<string>();
            if (!string.IsNullOrEmpty(task.Status))
            {
                statusList.Add(task.Status.ToString());
            }
            List<SelectListItem> allLabels = GetAllLabel();
            ViewBag.PossibleLabels = allLabels;


            GetAllStatus(task);

            // MakeNotificationReadonly();


            return PartialView(task);
        }

        [HttpPost]
        public PartialViewResult CreateFromKanban(Task task)
        {

            task.CreatedBy = (int)Membership.GetUser().ProviderUserKey;
            task.ModifiedBy = (int)Membership.GetUser().ProviderUserKey;
            task.CreateDate = DateTime.Now;
            task.ModificationDate = DateTime.Now;
            task.ActionDate = DateTime.Now;
            task.IsActive = true;
            if (ModelState.IsValid)
            {
                AddAssignUser(task);
                AddFollower(task);
                AddLabel(task);

                TaskPropertyChange Changed = unitOfWork.TaskRepository.InsertOrUpdate(task);
                unitOfWork.Save();
                if (task.ParentTaskId != null)
                {
                    SaveNotification(task, true, true, Changed);
                }
                else
                {
                    SaveNotification(task, true, false, Changed);
                }
            }

            List<SelectListItem> allUsers = GetAllUser(task.ProjectID);
            ViewBag.PossibleUsers = allUsers;
            ViewBag.PossibleProjects = unitOfWork.ProjectRepository.All;
            ViewBag.PossiblePriorities = unitOfWork.PriorityRepository.All;
            ViewBag.PossibleSprints = unitOfWork.SprintRepository.AllByProjectID(task.ProjectID);

            List<SelectListItem> allLabels = GetAllLabel();
            ViewBag.PossibleLabels = allLabels;
            GetAllStatus(task);

            Project project = unitOfWork.ProjectRepository.Find(task.ProjectID);
            ViewBag.CurrentProject = project;
            List<string> statusList = new List<string>();
            if (!string.IsNullOrEmpty(project.allStatus))
            {
                statusList = project.allStatus.Split(',').ToList();
            }
            ViewBag.AllStatus = statusList;
            List<Task> taskList = new List<Task>();
            taskList = unitOfWork.TaskRepository.ByProjectIncluding(task.ProjectID, t => t.Project).Include(t => t.Priority).Include(t => t.ChildTask).Include(t => t.Users).Include(t => t.Followers).Include(t => t.Labels).ToList();

            // return RedirectToAction("Kanban", new { @ProjectID = task.ProjectID });
            return PartialView("_Kanban", taskList);
        }

        private void MakeNotificationReadonly()
        {
            try
            {
                if (Request.QueryString["notificationID"] != null)
                {
                    Notification notification = unitOfWork.NotificationRepository.Find(Convert.ToInt64(Request.QueryString["notificationID"].ToString()));
                    notification.IsNoticed = true;
                    unitOfWork.NotificationRepository.InsertOrUpdate(notification);
                    unitOfWork.Save();
                    UserProfile user = unitOfWork.UserRepository.GetUserByUserID((int)Membership.GetUser(WebSecurity.CurrentUserName).ProviderUserKey);
                    LoadUnreadNotifications(user);
                }
            }
            catch
            {
            }
        }

        private List<SelectListItem> GetAllLabel()
        {
            List<SelectListItem> allLabels = new List<SelectListItem>();
            List<Label> labelList = unitOfWork.LabelRepository.All.ToList();
            foreach (Label label in labelList)
            {
                SelectListItem item = new SelectListItem { Value = label.LabelID.ToString(), Text = label.Name };
                allLabels.Add(item);
            }
            return allLabels;
        }
        private List<SelectListItem> GetallSprints(long projectID)
        {

            List<SelectListItem> allStatus = new List<SelectListItem>();
            List<Sprint> pList = unitOfWork.ProjectRepository.FindIncludingProjectStatus(projectID).Sprints;
            foreach (Sprint status in pList)
            {

                SelectListItem item = new SelectListItem { Value = status.SprintID.ToString(), Text = status.Name };
                allStatus.Add(item);
            }

            return allStatus;
        }

        private List<SelectListItem> GetallPriority()
        {

            List<SelectListItem> allStatus = new List<SelectListItem>();
            List<Priority> pList = unitOfWork.PriorityRepository.All.ToList();
            foreach (Priority status in pList)
            {

                SelectListItem item = new SelectListItem { Value = status.PriorityID.ToString(), Text = status.Name };
                allStatus.Add(item);
            }

            return allStatus;
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

        private List<SelectListItem> GetAllUser(long ProjectID)
        {
            List<SelectListItem> allUsers = new List<SelectListItem>();
            List<UserProfile> userList = unitOfWork.ProjectRepository.Find(ProjectID).Users;
            foreach (UserProfile user in userList)
            {
                SelectListItem item = new SelectListItem { Value = user.UserId.ToString(), Text = user.FirstName + " " + user.LastName };
                allUsers.Add(item);
            }
            return allUsers;
        }

        //
        // POST: /Tasks/Edit/5

        [HttpPost]
        public ActionResult Edit(Task task)
        {
            task.ModifiedBy = (int)Membership.GetUser().ProviderUserKey;
            task.ModificationDate = DateTime.Now;
            task.ActionDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                AddAssignUser(task);
                AddFollower(task);
                AddLabel(task);

                TaskPropertyChange Change = unitOfWork.TaskRepository.InsertOrUpdate(task);
                unitOfWork.Save();
                if (task.ParentTaskId != null)
                {
                    SaveNotification(task, false, false, Change);
                }
                else
                {
                    SaveNotification(task, false, true, Change);
                }
                return RedirectToAction("ProjectTasks", new { @ProjectID = task.ProjectID });
            }

            List<SelectListItem> allUsers = GetAllUser(task.ProjectID);
            ViewBag.PossibleUsers = allUsers;
            ViewBag.PossibleProjects = unitOfWork.ProjectRepository.All;
            ViewBag.PossiblePriorities = unitOfWork.PriorityRepository.All;
            ViewBag.PossibleSprints = unitOfWork.SprintRepository.AllByProjectID(task.ProjectID);

            List<SelectListItem> allLabels = GetAllLabel();
            ViewBag.PossibleLabels = allLabels;
            GetAllStatus(task);
            return View(task);
        }

        //
        // GET: /Tasks/Delete/5

        public ActionResult Delete(long id)
        {
            Task task = unitOfWork.TaskRepository.Find(id);
            return View(task);
        }

        //
        // POST: /Tasks/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            Task task = unitOfWork.TaskRepository.Find(id);
            unitOfWork.TaskRepository.Delete(id);
            unitOfWork.Save();
            return RedirectToAction("ProjectTasks", new { @ProjectID = task.ProjectID });
        }



        public ActionResult Kanban(long ProjectID)
        {
            List<Task> tasklist = unitOfWork.TaskRepository.ByProjectIncluding(ProjectID, task => task.Project).Include(task => task.Priority).Include(task => task.ChildTask).Include(task => task.Users).Include(task => task.Followers).Include(task => task.Labels).ToList();
            Project project = unitOfWork.ProjectRepository.Find(ProjectID);
            ViewBag.CurrentProject = project;
            List<string> statusList = new List<string>();
            if (!string.IsNullOrEmpty(project.allStatus))
            {
                statusList = project.allStatus.Split(',').ToList();
            }
            ViewBag.AllStatus = statusList;
            return View(tasklist);
        }

        public PartialViewResult _Kanban(long ProjectID)
        {
            List<Task> tasklist = unitOfWork.TaskRepository.ByProjectIncluding(ProjectID, task => task.Project).Include(task => task.Priority).Include(task => task.ChildTask).Include(task => task.Users).Include(task => task.Followers).Include(task => task.Labels).ToList();
            Project project = unitOfWork.ProjectRepository.FindincludingSprint(ProjectID);
            ViewBag.CurrentProject = project;
            List<string> statusList = new List<string>();
            if (!string.IsNullOrEmpty(project.allStatus))
            {
                statusList = project.allStatus.Split(',').ToList();
            }
            ViewBag.AllStatus = statusList;
            return PartialView(tasklist);
        }

        [HttpPost]
        public ActionResult Kanban(string taskid, string statusid, string sprintid)
        {
            Task task = new Task();
            string ststus = "";
            string logComment = "";
            try
            {
                long a = Convert.ToInt64(sprintid);
            }
            catch
            {
                sprintid = "";
            }
            if (taskid != null && statusid != null)
            {
                try
                {
                    task = unitOfWork.TaskRepository.Find(Convert.ToInt64(taskid));
                    string status = "";
                    string sprint = "";
                    if (!string.IsNullOrEmpty(statusid.Trim()))
                    {
                        if (task.ProjectStatusID != Convert.ToInt64(statusid))
                        {
                            if (statusid.Trim() == "")
                            {
                                status = "Unassigned";
                                task.Status = string.Empty;
                                task.ProjectStatusID = null;
                            }
                            else
                            {
                                status = unitOfWork.ProjectStatusRepository.Find(Convert.ToInt64(statusid)).Name;
                                task.ProjectStatusID = Convert.ToInt64(statusid);
                            }

                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(status.Trim()) && string.IsNullOrEmpty(sprintid.Trim()))
                        {
                            status = "Project Backlog";
                        }
                        else if (string.IsNullOrEmpty(status.Trim()) && !string.IsNullOrEmpty(sprintid.Trim()))
                        {
                            status = unitOfWork.SprintRepository.Find(Convert.ToInt64(sprintid)).Name + " Backlog";
                        }
                        task.Status = string.Empty;
                        task.ProjectStatusID = null;
                    }

                    if (sprintid.Trim() == "")
                    {
                        task.SprintID = null;
                    }
                    else
                    {
                        sprint = " under the sprint: " + unitOfWork.SprintRepository.Find(Convert.ToInt64(sprintid)).Name;
                        task.SprintID = Convert.ToInt64(sprintid);
                    }
                    unitOfWork.TaskRepository.InsertOrUpdate(task);
                    unitOfWork.Save();
                    ststus = "Task- " + task.Title + " is moved to " + status + sprint;
                    UserProfile user = unitOfWork.UserRepository.GetUserByUserID((int)Membership.GetUser(WebSecurity.CurrentUserName).ProviderUserKey);
                    logComment = ststus + "by user" + user.FirstName + "  " + user.LastName;
                    logComment = logComment + "on " + DateTime.Now.ToShortDateString();
                    status = status + " successfully!!!";
                }
                catch
                {
                    ststus = "Something Wrong!!!";
                }
                try
                {
                    TaskActivityLog log = unitOfWork.TaskActivityLogRepository.FindByTaskID(task.TaskID);
                    if (log == null)
                    {
                        TaskActivityLog logNew = new TaskActivityLog();
                        logNew.TaskID = task.TaskID;
                        logNew.Comment = logComment;
                        logNew.CreateDate = DateTime.Now;
                        logNew.ModificationDate = DateTime.Now;
                        unitOfWork.TaskActivityLogRepository.InsertOrUpdate(logNew);
                        unitOfWork.Save();
                    }
                    else
                    {
                        log.Comment = log.Comment + " ; " + logComment;
                        unitOfWork.Save();
                    }
                }
                catch
                {
                }
                return Content(ststus);
            }
            return Content(ststus);
        }



        [HttpPost]
        public PartialViewResult RemoveStatusFormKanban(long status, long projectID)
        {
            List<Task> taskList = new List<Task>();
            Project projectOld = unitOfWork.ProjectRepository.Find(projectID);

            taskList = unitOfWork.TaskRepository.ByProjectAndStatusIncluding(projectID, status, task => task.Project).Include(task => task.Priority).Include(task => task.ChildTask).Include(task => task.Users).Include(task => task.Followers).Include(task => task.Labels).ToList();
            if (!taskList.Any(e => e.ProjectStatusID == status))
            {
                ViewBag.isSattusDeleted = true;
                unitOfWork.ProjectStatusRepository.DeleteByProjectIDAndColID(status, projectID);
                unitOfWork.Save();
            }
            taskList = unitOfWork.TaskRepository.ByProjectIncluding(projectID, task => task.Project).Include(task => task.Priority).Include(task => task.ChildTask).Include(task => task.Users).Include(task => task.Followers).Include(task => task.Labels).ToList();

            ViewBag.CurrentProject = unitOfWork.ProjectRepository.Find(projectID);
            return PartialView("_Kanban", taskList);

        }

        public PartialViewResult CreateStatus(long id)
        {

            ProjectStatus projectCol = new ProjectStatus();
            projectCol.ProjectID = id;
            projectCol.color = "#3B6998";
            return PartialView(projectCol);
        }

        [HttpPost]
        public PartialViewResult CreateStatus(ProjectStatus projectcol)
        {

            unitOfWork.ProjectStatusRepository.InsertOrUpdate(projectcol);
            unitOfWork.Save();
            Project project = new Project();
            //List<Task> taskList = unitOfWork.TaskRepository.ByProjectIncluding(projectcol.ProjectID, task => task.Project).Include(task => task.Priority).Include(task => task.ChildTask).Include(task => task.Users).Include(task => task.Followers).Include(task => task.Labels).ToList();
            ViewBag.CurrentProject = unitOfWork.ProjectRepository.Find(projectcol.ProjectID);
            Project projectOld = unitOfWork.ProjectRepository.Find(projectcol.ProjectID);
            List<Task> taskList = new List<Task>();
            try
            {
                projectOld.allStatus = project.allStatus;
                unitOfWork.ProjectRepository.InsertOrUpdate(projectOld);
                unitOfWork.Save();
                taskList = unitOfWork.TaskRepository.ByProjectIncluding(project.ProjectID, task => task.Project).Include(task => task.Priority).Include(task => task.ChildTask).Include(task => task.Users).Include(task => task.Followers).Include(task => task.Labels).ToList();
                foreach (Task task in taskList)
                {
                    task.Status = string.Empty;
                    unitOfWork.Save();
                }
                ViewBag.CurrentProject = projectOld;
                List<string> statusList = new List<string>();
                if (!string.IsNullOrEmpty(project.allStatus))
                {
                    statusList = project.allStatus.Split(',').ToList();
                }
                ViewBag.AllStatus = statusList;
            }
            catch
            {
            }
            taskList = unitOfWork.TaskRepository.ByProjectIncluding(projectcol.ProjectID, task => task.Project).Include(task => task.Priority).Include(task => task.ChildTask).Include(task => task.Users).Include(task => task.Followers).Include(task => task.Labels).ToList();
            return PartialView("_Kanban", taskList);
        }

        public PartialViewResult EditStatus(long status, long projectID)
        {
            ProjectStatus projectCol = unitOfWork.ProjectStatusRepository.FindbyProjectIDAndProjectStatusID(projectID, status);
            return PartialView(projectCol);
        }

        [HttpPost]
        public PartialViewResult EditStatus(ProjectStatus projectcol)
        {
            unitOfWork.ProjectStatusRepository.InsertOrUpdate(projectcol);
            unitOfWork.Save();
            List<Task> taskList = unitOfWork.TaskRepository.ByProjectIncluding(projectcol.ProjectID, task => task.Project).Include(task => task.Priority).Include(task => task.ChildTask).Include(task => task.Users).Include(task => task.Followers).Include(task => task.Labels).ToList();
            ViewBag.CurrentProject = unitOfWork.ProjectRepository.Find(projectcol.ProjectID);
            return PartialView("_Kanban", taskList);
        }



        public ActionResult CreateSprintFromKanban(long id)
        {
            Sprint sprint = new Sprint();
            sprint.StartDate = DateTime.Now;
            sprint.EndDate = DateTime.Now;
            sprint.ProjectID = id;
            sprint.Project = unitOfWork.ProjectRepository.Find(sprint.ProjectID);

            return PartialView(sprint);
        }

        [HttpPost]
        public PartialViewResult CreateSprintFromKanban(Sprint sprint)
        {

            if (ModelState.IsValid)
            {
                unitOfWork.SprintRepository.InsertOrUpdate(sprint);
                //Ahange by Mahedee @06-03-14. Because is active check box is removed from UI
                sprint.IsActive = true;
                unitOfWork.Save();
            }

            Project project = unitOfWork.ProjectRepository.Find(sprint.ProjectID);
            ViewBag.CurrentProject = project;
            List<Task> taskList = new List<Task>();
            //taskList = unitOfWork.TaskRepository.GetTasksBySprintID(sprint.SprintID);

            taskList = unitOfWork.TaskRepository.ByProjectIncluding(sprint.ProjectID, task => task.Project).Include(task => task.Priority).Include(task => task.ChildTask).Include(task => task.Users).Include(task => task.Followers).Include(task => task.Labels).ToList();
            return PartialView("_Kanban", taskList);
        }

        public PartialViewResult Activity(long taskID)
        {
            TaskActivityLog log = new TaskActivityLog();
            log.TaskID = taskID;
            ViewBag.CurrentTask = unitOfWork.TaskRepository.Find(taskID);
            ViewBag.PossibleActivities = unitOfWork.TaskActivityLogRepository.AllAttachmentByTaskID(taskID);
            return PartialView(log);
        }

        public PartialViewResult ShowLog(long taskID)
        {
            TaskActivityLog log = new TaskActivityLog();
            log = unitOfWork.TaskActivityLogRepository.AllActivityByTaskID(taskID);
            return PartialView(log);
        }

        [HttpPost]
        public virtual ContentResult ActivityAdd(long taskID)
        {
            string content = "";
            if (ModelState.IsValid)
            {
                string comment = "";
                UserProfile user = unitOfWork.UserRepository.GetUserByUserID((int)Membership.GetUser(WebSecurity.CurrentUserName).ProviderUserKey);
                string Name = "";
                string Length = "";
                string Type = "";
                foreach (string file in Request.Files)
                {
                    HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                    if (hpf.ContentLength == 0)
                        continue;
                    comment = user.FirstName + " " + user.LastName + " Added a file " + Path.GetFileName(hpf.FileName);


                    TaskActivityLog log = new TaskActivityLog();
                    log.TaskID = taskID;
                    log.CreateDate = DateTime.Now;
                    log.ModificationDate = DateTime.Now;
                    log.Comment = comment;
                    log.FileDisplayName = Path.GetFileName(hpf.FileName);
                    unitOfWork.TaskActivityLogRepository.InsertOrUpdate(log);
                    unitOfWork.Save();

                    var path = Server.MapPath("~/UploadedDocument/") + "T-" + log.TaskID.ToString() + "L-" + log.TaskActivityLogID.ToString() + Path.GetExtension(hpf.FileName);
                    log.FileUrl = "~/UploadedDocument/" + "T-" + log.TaskID.ToString() + "L-" + log.TaskActivityLogID.ToString() + Path.GetExtension(hpf.FileName);
                    unitOfWork.TaskActivityLogRepository.InsertOrUpdate(log);
                    unitOfWork.Save();
                    hpf.SaveAs(Server.MapPath(log.FileUrl));


                    Name = hpf.FileName;
                    Length = hpf.ContentLength.ToString();
                    Type = hpf.ContentType;
                    content = "{\"name\":\"" + Name + "\",\"type\":\"" + Type + "\",\"size\":\"" + string.Format("{0} bytes", Length) + "\"}";
                }

            }
            return Content(content, "application/json");
        }


        public PartialViewResult _Search(long projectID)
        {
            ViewBag.CurrentProjectId = projectID;
            Project project = unitOfWork.ProjectRepository.Find(projectID);
            Search search = new Search();
            search.statusList = unitOfWork.ProjectStatusRepository.FindbyProjectID(projectID);
            search.priorityList = unitOfWork.PriorityRepository.All.ToList();
            search.SprintList = project.Sprints.ToList();
            if (project.Users != null)
            {
                foreach (UserProfile user in project.Users)
                {
                    search.UserList.Add(user);
                }
            }
            if (project.ProjectOwners != null)
            {
                foreach (UserProfile user in project.ProjectOwners)
                {
                    if (!search.UserList.Exists(u => u.UserId == user.UserId))
                        search.UserList.Add(user);
                }
            }

            return PartialView("_Search", search);
        }

        [HttpPost]
        public ActionResult _Search(Search search)
        {
            List<Task> taskList = new List<Task>();
            long statusId = Convert.ToInt64(search.SelectedStatusID);

            long projectID = (long)search.SelectedProjectID;
            taskList = GetTasks(search);
            ViewBag.Tasks = taskList;
            return RedirectToAction("_TaskList", new { @ProjectID = projectID, @statusId = statusId });

        }

        private List<Task> GetTasks(Search search)
        {
            return unitOfWork.TaskRepository.GetBySearchCriteria(search);
        }

        public List<Task> GetTasks(long projectID, long statusId)
        {

            List<Task> taskList = new List<Task>();
            UserProfile user = unitOfWork.UserRepository.GetUserByUserID((int)Membership.GetUser(WebSecurity.CurrentUserName).ProviderUserKey);
            Project project = unitOfWork.ProjectRepository.Find(projectID);
            //If this project is created by the current user. Then he can see all task.
            if (project.CreatedBy == user.UserId)
                taskList = unitOfWork.TaskRepository.ByProjectIncluding(projectID, task => task.Project).Include(task => task.Priority).Include(task => task.ChildTask).Include(task => task.Users).Include(task => task.Followers).Include(task => task.Labels).Where(p => p.ProjectStatus.ProjectStatusID == statusId).ToList();

            //If this project is owned by the current user. Then he can see all task.
            else if (project.ProjectOwners.Contains(user))
                taskList = unitOfWork.TaskRepository.ByProjectIncluding(projectID, task => task.Project).Include(task => task.Priority).Include(task => task.ChildTask).Include(task => task.Users).Include(task => task.Followers).Include(task => task.Labels).Where(p => p.ProjectStatus.ProjectStatusID == statusId).ToList();

            else
                taskList = unitOfWork.TaskRepository.ByProjectIncluding(projectID, user, task => task.Project).Include(task => task.Priority).Include(task => task.ChildTask).Include(task => task.Users).Include(task => task.Followers).Include(task => task.Labels).Where(p => p.ProjectStatus.ProjectStatusID == statusId).ToList();




            return taskList;
        }

        public PartialViewResult _KanbanTaskDetail(long taskID)
        {
            Task task = unitOfWork.TaskRepository.Find(taskID);
            return PartialView(task);

        }
        public PartialViewResult StatusSwapForm(long id)
        {

            List<ProjectStatus> statusList = unitOfWork.ProjectStatusRepository.FindbyProjectIDWithoutUnmovable(id);
            ViewBag.CurrentProjectID = id;
            return PartialView(statusList);

        }

        [HttpPost]
        public PartialViewResult StatusSwapForm(ProjectStatus[] Statuses)
        {
            long projectID = Convert.ToInt64(Request.Form["CurrentProjectID"].ToString());
            List<Task> taskList = unitOfWork.TaskRepository.ByProjectIncluding(projectID, task => task.Project).Include(task => task.Priority).Include(task => task.ChildTask).Include(task => task.Users).Include(task => task.Followers).Include(task => task.Labels).ToList();
            ViewBag.CurrentProject = unitOfWork.ProjectRepository.Find(projectID);
            return PartialView("_Kanban", taskList);
        }
        [HttpPost]
        public ContentResult SwapStaus(string sortorder)
        {
            List<string> orders = sortorder.Split(',').ToList();
            int i = 1;
            foreach (string order in orders)
            {
                if (order != "")
                {
                    ProjectStatus status = unitOfWork.ProjectStatusRepository.Find(Convert.ToInt64(order));
                    status.SlNo = i;
                    i++;
                }
            }
            unitOfWork.Save();
            return Content("");
        }

        public PartialViewResult RulesForm(long id)
        {
            ProjectStatusRule rule = new ProjectStatusRule();
            rule.ProjectID = id;
            ViewBag.CurrentProjectStatuses = unitOfWork.ProjectStatusRepository.FindbyProjectID(id);
            ViewBag.Rules = unitOfWork.ProjectStatusRuleRepository.FindbyProjectID(id);
            return PartialView(rule);
        }



        [HttpPost]
        public PartialViewResult RulesForm(ProjectStatusRule rule)
        {
            unitOfWork.ProjectStatusRuleRepository.InsertOrUpdate(rule);
            unitOfWork.Save();
            ViewBag.CurrentProjectStatuses = unitOfWork.ProjectStatusRepository.FindbyProjectID(rule.ProjectID);
            ViewBag.Rules = unitOfWork.ProjectStatusRuleRepository.FindbyProjectID(rule.ProjectID);
            return PartialView(rule);
        }



        public PartialViewResult TaskDetailForm(long id)
        {
            Task task = unitOfWork.TaskRepository.Find(id);

            return PartialView(task);
        }

        public PartialViewResult DeleteRule(long id)
        {
            ProjectStatusRule deletedrule = unitOfWork.ProjectStatusRuleRepository.Find(id);

            unitOfWork.ProjectStatusRuleRepository.Delete(id);
            unitOfWork.Save();
            ViewBag.CurrentProjectStatuses = unitOfWork.ProjectStatusRepository.FindbyProjectID(deletedrule.ProjectID);
            ViewBag.Rules = unitOfWork.ProjectStatusRuleRepository.FindbyProjectID(deletedrule.ProjectID);

            ProjectStatusRule rule = new ProjectStatusRule();
            rule.ProjectID = deletedrule.ProjectID;
            return PartialView(rule);
        }



        public PartialViewResult AddMessage(long id)
        {
            Task task = unitOfWork.TaskRepository.Find(id);
            ViewBag.AllMessage = unitOfWork.TaskMessageRepository.FindAllByTask(id);

            ViewBag.PossibleUser = GetAllUser(task.ProjectID);
            TaskMessage taskMessage = new TaskMessage();
            taskMessage.TaskID = id;
            taskMessage.FormUserID = (int)Membership.GetUser(WebSecurity.CurrentUserName).ProviderUserKey;
            taskMessage.CreateDate = DateTime.Now;
            return PartialView(taskMessage);
        }

        [HttpPost]
        public PartialViewResult AddMessage(TaskMessage taskMessage)
        {
            if (taskMessage.SelectedToUsers != null)
            {
                foreach (string userid in taskMessage.SelectedToUsers)
                {
                    TaskMessage newTaskMessage = new TaskMessage();
                    newTaskMessage.TaskID = taskMessage.TaskID;
                    newTaskMessage.FormUserID = taskMessage.FormUserID;
                    newTaskMessage.CreateDate = taskMessage.CreateDate;
                    newTaskMessage.Message = taskMessage.Message;
                    newTaskMessage.ToUserID = Convert.ToInt32(userid);
                    unitOfWork.TaskMessageRepository.InsertOrUpdate(newTaskMessage);
                }
                unitOfWork.Save();
            }
            else
            {
                ModelState.AddModelError("CustomError", "Please select user");
            }
            Task task = unitOfWork.TaskRepository.Find(taskMessage.TaskID);
            ViewBag.AllMessage = unitOfWork.TaskMessageRepository.FindAllByTask(taskMessage.TaskID);

            ViewBag.PossibleUser = GetAllUser(task.ProjectID);
            TaskMessage taskMessageNext = new TaskMessage();
            taskMessageNext.TaskID = taskMessage.TaskID;
            taskMessageNext.FormUserID = (int)Membership.GetUser(WebSecurity.CurrentUserName).ProviderUserKey;
            taskMessageNext.CreateDate = DateTime.Now;
            taskMessageNext.SelectedToUsers = new List<string>();
            taskMessageNext.Message = "";
            return PartialView(taskMessageNext);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}