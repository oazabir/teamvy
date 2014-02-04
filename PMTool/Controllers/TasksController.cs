using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PMTool.Models;
using PMTool.Repository;

namespace PMTool.Controllers
{
    public class TasksController : BaseController
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        //
        // GET: /Tasks/

        public ViewResult Index()
        {
            return View(unitOfWork.TaskRepository.AllIncluding(task => task.Project).Include(task => task.Priority).Include(task => task.ChildTask).Include(task => task.Users).Include(task => task.Followers).Include(task => task.Labels).ToList());
        }

        //
        // GET: /Tasks/ProjectTasks?ProjectID=5
        public ViewResult ProjectTasks(long projectID)
        {
            List<Task> taskList = unitOfWork.TaskRepository.ByProjectIncluding(projectID, task => task.Project).Include(task => task.Priority).Include(task => task.ChildTask).Include(task => task.Users).Include(task => task.Followers).Include(task => task.Labels).ToList();
            ViewBag.CurrentProject = unitOfWork.ProjectRepository.Find(projectID);
            return View(taskList);
        }

        //
        // GET: /Tasks/SubTaskList?taskID=5
        public ViewResult SubTaskList(long projectID,long taskID)
        {
            List<Task> taskList = unitOfWork.TaskRepository.AllSubTaskByProjectIncluding(projectID,taskID, task => task.Project).Include(task => task.Priority).Include(task => task.ChildTask).Include(task => task.Users).Include(task => task.Followers).Include(task => task.Labels).ToList();
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

        //
        // GET: /Tasks/Create?ProjectID=5

        public ActionResult Create(long ProjectID)
        {
            List<SelectListItem> allUsers = GetAllUser(ProjectID);
            ViewBag.PossibleUsers = allUsers;

            ViewBag.PossibleProjects = unitOfWork.ProjectRepository.All;
            ViewBag.PossiblePriorities = unitOfWork.PriorityRepository.All;

            List<SelectListItem> allLabels = GetAllLabel();
            ViewBag.PossibleLabels = allLabels;
            Task task = new Task();
            task.ProjectID = ProjectID;
            task.StartDate = DateTime.Now;
            task.EndDate = DateTime.Now;

            GetAllStatus(task);

            return View(task);
        }

        private void GetAllStatus(Task task)
        {
            List<SelectListItem> statusList = new List<SelectListItem>();
            foreach (var item in Enum.GetValues(typeof(PMTool.Models.EnumCollection.TaskStatus)))
            {
                SelectListItem listitem = new SelectListItem();

                listitem.Value = Convert.ToString((int)item);
                listitem.Text = Enum.GetName(typeof(PMTool.Models.EnumCollection.TaskStatus), item).Replace("_", " ");
                if ((int)item == (int)task.Status)
                {
                    listitem.Selected = true;
                }
                else
                {
                    listitem.Selected = false;
                }
                statusList.Add(listitem);
            }

            ViewBag.PossibleTaskStatus = statusList;
        }

        //
        // GET: /Tasks/CreateSubTask?ProjectID=5&TaskID=1

        public ActionResult CreateSubTask(long ProjectID,long TaskID)
        {
            List<SelectListItem> allUsers = GetAllUser(ProjectID);
            ViewBag.PossibleUsers = allUsers;
            ViewBag.PossibleProjects = unitOfWork.ProjectRepository.All;
            ViewBag.PossiblePriorities = unitOfWork.PriorityRepository.All;

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
            task.CreatedBy = (Guid)Membership.GetUser().ProviderUserKey;
            task.ModifieddBy = (Guid)Membership.GetUser().ProviderUserKey;
            task.CreateDate = DateTime.Now;
            task.ModificationDate = DateTime.Now;
            task.ActionDate = DateTime.Now;
            task.IsActive = true;
            if (ModelState.IsValid)
            {
              bool isStatusChanged=  unitOfWork.TaskRepository.InsertOrUpdate(task);
                AddAssignUser(task);
                AddFollower(task);
                AddLabel(task);
                unitOfWork.Save();
                SaveNotification(task, true, false, isStatusChanged);
                return RedirectToAction("ProjectTasks", new { @ProjectID = task.ProjectID });
            }
            List<SelectListItem> allUsers = GetAllUser(task.ProjectID);
            ViewBag.PossibleUsers = allUsers;
            ViewBag.PossibleProjects = unitOfWork.ProjectRepository.All;
            ViewBag.PossiblePriorities = unitOfWork.PriorityRepository.All;

            List<SelectListItem> allLabels = GetAllLabel();
            ViewBag.PossibleLabels = allLabels;
            GetAllStatus(task);
            return View(task); //return RedirectToAction("ProjectTasks", new { @ProjectID = task.ProjectID });
        }

        private void SaveNotification(Task task, bool isTaskInsert,bool isSubTask,bool isTaskStatusChanged)
        {
            string phrase = "";
            if (task.Users != null)
            {
                foreach (User user in task.Users)
                {
                    Notification notification = new Notification();
                    if (isTaskInsert)
                    {
                        if (!isSubTask)
                        {
                            phrase = " Has assigned you on the Task --";
                        }
                        else
                        {
                            phrase = " Has assigned you on the Subtask --";
                        }

                        User createdUser = unitOfWork.UserRepository.GetUserByUserID(task.CreatedBy);
                        notification.Title = createdUser.FirstName + " " + createdUser.LastName + phrase + task.Title;
                    }
                    else
                    {
                        if (!isTaskStatusChanged)
                        {
                            if (!isSubTask)
                            {
                                phrase = " Has modify the Task --";
                            }
                            else
                            {
                                phrase = " Has modify the Subtask --";
                            }
                        }
                        else
                        {
                            if (!isSubTask)
                            {
                                phrase = string.Format(" Has changed the task status to {0} --",task.Status.ToString());
                            }
                            else
                            {
                                phrase = string.Format(" Has changed the subtask status to {0} --",task.Status.ToString());
                            }
                        }
                        User modifiedUser = unitOfWork.UserRepository.GetUserByUserID(task.ModifieddBy);
                        notification.Title = modifiedUser.FirstName + " " + modifiedUser.LastName + phrase + task.Title;
                    }
                    notification.UserID = user.UserId;
                    notification.Description = notification.Title;
                    notification.ProjectID = task.ProjectID;
                    notification.TaskID = task.TaskID;
                    unitOfWork.NotificationRepository.InsertOrUpdate(notification);
                    unitOfWork.Save();
                }
            }
            if (task.Followers != null)
            {
                foreach (User user in task.Followers)
                {
                    Notification notification = new Notification();
                    if (isTaskInsert)
                    {
                        if (!isSubTask)
                        {
                            phrase = " Has added you as a follower on the Task --";
                        }
                        else
                        {
                            phrase = " Has added you as a follower on the Subtask --";
                        }
                        User createdUser = unitOfWork.UserRepository.GetUserByUserID(task.CreatedBy);
                        notification.Title = createdUser.FirstName + " " + createdUser.LastName + phrase + task.Title;
                    }
                    else
                    {
                        if (!isSubTask)
                        {
                            phrase = " Has modify the Task --";
                        }
                        else
                        {
                            phrase = " Has modify the Subtask --";
                        }
                        User modifiedUser = unitOfWork.UserRepository.GetUserByUserID(task.ModifieddBy);
                        notification.Title = modifiedUser.FirstName + " " + modifiedUser.LastName + phrase + task.Title;
                    }
                    notification.UserID = user.UserId;
                    notification.Description = notification.Title;
                    notification.ProjectID = task.ProjectID;
                    notification.TaskID = task.TaskID;
                    unitOfWork.NotificationRepository.InsertOrUpdate(notification);
                    unitOfWork.Save();
                }
            }
        }

        //
        // POST: /Tasks/CreateSubTask

        [HttpPost]
        public ActionResult CreateSubTask(Task task)
        {
            task.TaskID = 0;
            task.CreatedBy = (Guid)Membership.GetUser().ProviderUserKey;
            task.ModifieddBy = (Guid)Membership.GetUser().ProviderUserKey;
            task.CreateDate = DateTime.Now;
            task.ModificationDate = DateTime.Now;
            task.ActionDate = DateTime.Now;
            if (ModelState.IsValid)
            {
               bool isStatusChanged= unitOfWork.TaskRepository.InsertOrUpdate(task);
                AddAssignUser(task);
                AddFollower(task);
                AddLabel(task);
                unitOfWork.Save();
                SaveNotification(task, true, true, isStatusChanged);
                return RedirectToAction("ProjectTasks", new { @ProjectID = task.ProjectID });
            }
            List<SelectListItem> allUsers = GetAllUser(task.ProjectID);
            ViewBag.PossibleUsers = allUsers;
            ViewBag.PossibleProjects = unitOfWork.ProjectRepository.All;
            ViewBag.PossiblePriorities = unitOfWork.PriorityRepository.All;

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
            task.Followers = new List<User>();
            if (task.SelectedFollowedUsers != null)
            {
                foreach (string userID in task.SelectedFollowedUsers)
                {
                    User user = unitOfWork.UserRepository.GetUserByUserID(new Guid(userID));
                    task.Followers.Add(user);
                }
            }
        }

        private void AddAssignUser(Task task)
        {
            task.Users = new List<User>();
            if (task.SelectedAssignedUsers != null)
            {
                foreach (string userID in task.SelectedAssignedUsers)
                {
                    User user = unitOfWork.UserRepository.GetUserByUserID(new Guid(userID));
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

            List<SelectListItem> allUsers = GetAllUser(task.ProjectID);
            ViewBag.PossibleUsers = allUsers;

            task.SelectedAssignedUsers = task.Users.Select(u => u.UserId.ToString()).ToList();
            task.SelectedFollowedUsers = task.Followers.Select(u => u.UserId.ToString()).ToList();
            task.SelectedLabels = task.Labels.Select(u => u.LabelID.ToString()).ToList();
            List<string> statusList = new List<string>();
            statusList.Add( task.Status.ToString());
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

            List<SelectListItem> allUsers = GetAllUser(task.ProjectID);
            ViewBag.PossibleUsers = allUsers;

            task.SelectedAssignedUsers = task.Users.Select(u => u.UserId.ToString()).ToList();
            task.SelectedFollowedUsers = task.Followers.Select(u => u.UserId.ToString()).ToList();
            task.SelectedLabels = task.Labels.Select(u => u.LabelID.ToString()).ToList();
            List<string> statusList = new List<string>();
            statusList.Add(task.Status.ToString());
            List<SelectListItem> allLabels = GetAllLabel();
            ViewBag.PossibleLabels = allLabels;


            GetAllStatus(task);

            MakeNotificationReadonly();


            return PartialView(task);
        }

        [HttpPost]
        public ActionResult EditFromKanban(Task task)
        {
            task.ModifieddBy = (Guid)Membership.GetUser().ProviderUserKey;
            task.ModificationDate = DateTime.Now;
            task.ActionDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                if (ValidTaskStatus(task))
                {
                    AddAssignUser(task);
                    AddFollower(task);
                    AddLabel(task);

                    bool isStatusChanged = unitOfWork.TaskRepository.InsertOrUpdate(task);
                    unitOfWork.Save();
                    if (task.ParentTaskId != null)
                    {
                        SaveNotification(task, false, true, isStatusChanged);
                    }
                    else
                    {
                        SaveNotification(task, false, false, isStatusChanged);
                    }
                    //return RedirectToAction("ProjectTasks", new { @ProjectID = task.ProjectID });
                }
                else
                {
                    TempData["Message"] = "One or more subtask is not closed yet!!!";
                }
            }

            List<SelectListItem> allUsers = GetAllUser(task.ProjectID);
            ViewBag.PossibleUsers = allUsers;
            ViewBag.PossibleProjects = unitOfWork.ProjectRepository.All;
            ViewBag.PossiblePriorities = unitOfWork.PriorityRepository.All;

            List<SelectListItem> allLabels = GetAllLabel();
            ViewBag.PossibleLabels = allLabels;
            GetAllStatus(task);
            return RedirectToAction("Kanban", new { @ProjectID = task.ProjectID });
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
                    User user = unitOfWork.UserRepository.GetUserByUserID((Guid)Membership.GetUser(WebSecurity.User.Identity.Name).ProviderUserKey);
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

        private List<SelectListItem> GetAllUser(long ProjectID)
        {
            List<SelectListItem> allUsers = new List<SelectListItem>();
            List<User> userList = unitOfWork.ProjectRepository.Find(ProjectID).Users;
            foreach (User user in userList)
            {
                SelectListItem item = new SelectListItem { Value = user.UserId.ToString(), Text = user.FirstName +" "+ user.LastName };
                allUsers.Add(item);
            }
            return allUsers;
        }

        //
        // POST: /Tasks/Edit/5

        [HttpPost]
        public ActionResult Edit(Task task)
        {
            task.ModifieddBy = (Guid)Membership.GetUser().ProviderUserKey;
            task.ModificationDate = DateTime.Now;
            task.ActionDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                if (ValidTaskStatus(task))
                {
                    AddAssignUser(task);
                    AddFollower(task);
                    AddLabel(task);

                  bool isStatusChanged=  unitOfWork.TaskRepository.InsertOrUpdate(task);
                    unitOfWork.Save();
                    if (task.ParentTaskId != null)
                    {
                        SaveNotification(task, false, false, isStatusChanged);
                    }
                    else
                    {
                        SaveNotification(task, false, true, isStatusChanged);
                    }
                    return RedirectToAction("ProjectTasks", new { @ProjectID = task.ProjectID });
                }
                else
                {
                    TempData["Message"] = "One or more subtask is not closed yet!!!";
                }
            }

            List<SelectListItem> allUsers = GetAllUser(task.ProjectID);
            ViewBag.PossibleUsers = allUsers;
            ViewBag.PossibleProjects = unitOfWork.ProjectRepository.All;
            ViewBag.PossiblePriorities = unitOfWork.PriorityRepository.All;

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
           Task task= unitOfWork.TaskRepository.Find(id);
            unitOfWork.TaskRepository.Delete(id);
            return RedirectToAction("ProjectTasks", new { @ProjectID = task.ProjectID });
        }


        private bool ValidTaskStatus(Task task)
        {
            bool isvalid = true;
            if (task.Status == PMTool.Models.EnumCollection.TaskStatus.Closed)
            {
                List<Task> subTaskList = unitOfWork.TaskRepository.AllSubTaskByProjectIncluding(task.ProjectID, task.TaskID, t => t.Project).Include(t => t.Priority).Include(t => t.ChildTask).Include(t => t.Users).Include(t => t.Followers).Include(t => t.Labels).ToList();
                if (subTaskList.Any(st => st.Status != PMTool.Models.EnumCollection.TaskStatus.Closed))
                {
                    isvalid = false;
                }
            }
            return isvalid;
        }

        public ActionResult Kanban(long ProjectID)
        {
            List<Task> tasklist = unitOfWork.TaskRepository.ByProjectIncluding(ProjectID, task => task.Project).Include(task => task.Priority).Include(task => task.ChildTask).Include(task => task.Users).Include(task => task.Followers).Include(task => task.Labels).ToList();
            ViewBag.CurrentProject = unitOfWork.ProjectRepository.Find(ProjectID);


            List<PMTool.Models.EnumCollection.TaskStatus> statusList = Enum.GetValues(typeof(PMTool.Models.EnumCollection.TaskStatus)).Cast<PMTool.Models.EnumCollection.TaskStatus>().ToList();
            ViewBag.AllStatus = statusList;
            return View(tasklist);
        }

        [HttpPost]
        public ActionResult Kanban(string taskid, string statusid)
        {
            string ststus = "";
            if (taskid != null && statusid != null)
            {
                try
                {
                    Task task = unitOfWork.TaskRepository.Find(Convert.ToInt64(taskid));
                    if ((int)task.Status != Convert.ToInt32(statusid))
                    {
                        task.Status = (PMTool.Models.EnumCollection.TaskStatus)Convert.ToInt32(statusid);
                        unitOfWork.TaskRepository.InsertOrUpdate(task);
                        unitOfWork.Save();
                        ststus = "Task- " + task.Title + " is moved to " + task.Status.ToString().Replace("_", " ") + " successfully!!!";
                    }
                }
                catch
                {

                }
                return Content(ststus);
            }
            return Content(ststus);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}