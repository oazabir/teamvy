using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMTool.Models;
using PMTool.Repository;
using System.Web.Security;

namespace PMTool.Controllers
{
    [Authorize]
    public class ProjectsController :BaseController
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        //
        // GET: /Projects/

        public ViewResult Index()
        {
            TempData["Message"] = TempData["Message"];
            return View(unitOfWork.ProjectRepository.AllbyUserIncluding((Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey, project => project.Users).ToList());
        }

        //
        // GET: /Projects/Details/5

        public ViewResult Details(long id)
        {
            Project project = unitOfWork.ProjectRepository.Find(id);
            MakeNotificationReadonly();
            return View(project);
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

        //
        // GET: /Projects/Create

        public ActionResult Create()
        {
            List<SelectListItem> allUsers = GetAllUser();
            ViewBag.PossibleUsers = allUsers;
            Project project = new Project();
            project.allStatus = "";
            return View(project);
        }

        /// <summary>
        /// Get All possible user for assigning project
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetAllUser()
        {
            List<SelectListItem> allUsers = new List<SelectListItem>();
            List<User> userList = unitOfWork.UserRepository.All();
            foreach (User user in userList)
            {
                SelectListItem item = new SelectListItem { Value = user.UserId.ToString(), Text = user.FirstName +" "+ user.LastName };
                allUsers.Add(item);
            }
            return allUsers;
        }

        //
        // POST: /Projects/Create

        [HttpPost]
        public ActionResult Create(Project project)
        {
            project.ModificationDate = DateTime.Now;
            project.CreateDate = DateTime.Now;
            project.ActionDate = DateTime.Now;
            project.CreatedBy = (Guid)Membership.GetUser().ProviderUserKey;
            project.ModifieddBy = (Guid)Membership.GetUser().ProviderUserKey;

            if (ModelState.IsValid)
            {
                unitOfWork.ProjectRepository.InsertOrUpdate(project);
                AddAssignUser(project);
                AddProjectOwner(project);
                unitOfWork.Save();
                SaveNotification(project,true);
                
                return RedirectToAction("Index");
            }
            List<SelectListItem> allUsers = GetAllUser();
            ViewBag.PossibleUsers = allUsers;
            return View(project);
        }

        private void SaveNotification(Project project,bool isProjectInsert)
        {
            if (project.Users != null)
            {
                foreach (User user in project.Users)
                {
                    

                    Notification notification = new Notification();
                    if (isProjectInsert)
                    {
                        User createdUser = unitOfWork.UserRepository.GetUserByUserID(project.CreatedBy);
                        notification.Title = createdUser.FirstName + " " + createdUser.LastName + " Has added you on the porject --" + project.Name;
                    }
                    else
                    {
                        User modifiedUser = unitOfWork.UserRepository.GetUserByUserID(project.ModifieddBy);
                        notification.Title = modifiedUser.FirstName + " " + modifiedUser.LastName + " Has modify the porject --" + project.Name;
                    }
                    notification.UserID = user.UserId;
                    notification.Description = notification.Title;
                    notification.ProjectID = project.ProjectID;
                    unitOfWork.NotificationRepository.InsertOrUpdate(notification);
                    unitOfWork.Save();
                }
            }
        }

        private void AddAssignUser(Project project)
        {
            project.Users = new List<User>();
            if (project.SelectedAssignedUsers != null)
            {
                foreach (string userID in project.SelectedAssignedUsers)
                {
                    User user = unitOfWork.UserRepository.GetUserByUserID(new Guid(userID));
                    project.Users.Add(user);

                }
            }
        }

        private void AddProjectOwner(Project project)
        {
            project.ProjectOwners = new List<User>();
            if (project.SelectedProjectsOwners != null)
            {
                foreach (string userID in project.SelectedProjectsOwners)
                {
                    User user = unitOfWork.UserRepository.GetUserByUserID(new Guid(userID));
                    project.ProjectOwners.Add(user);
                }
            }
        }

        //
        // GET: /Projects/Edit/5

        public ActionResult Edit(long id)
        {
            Project project = unitOfWork.ProjectRepository.Find(id);
            List<SelectListItem> allUsers = GetAllUser();
            ViewBag.PossibleUsers = allUsers;
            project.SelectedAssignedUsers = project.Users.Select(u => u.UserId.ToString()).ToList();
            project.SelectedProjectsOwners = project.ProjectOwners.Select(u => u.UserId.ToString()).ToList();
            return View(project);
        }

        //
        // POST: /Projects/Edit/5

        [HttpPost]
        public ActionResult Edit(Project project)
        {
            project.ModificationDate = DateTime.Now;
            project.ActionDate = DateTime.Now;
            project.ModifieddBy = (Guid)Membership.GetUser().ProviderUserKey;
            List<User> userList = new List<User>();
            if (ModelState.IsValid)
            {
                AddAssignUser(project);
                AddProjectOwner(project);
               userList= unitOfWork.ProjectRepository.InsertOrUpdate(project);
                unitOfWork.Save();
                SaveNotification(project, false);
                string msg = "";
                foreach (User user in userList)
                {
                    msg = msg + user.FirstName + " " + user.LastName + ", ";
                }
                msg = msg + " user(s) are assigned in one or more task in this project;";
                if (msg != string.Empty)
                {
                    TempData["Message"]= msg;
                }
                return RedirectToAction("Index");
                
            }
            List<SelectListItem> allUsers = GetAllUser();
            ViewBag.PossibleUsers = allUsers;
           // project.SelectedAssignedUsers = project.Users.Select(u => u.UserId.ToString()).ToList();
            return View(project);

        }

        //
        // GET: /Projects/Delete/5

        public ActionResult Delete(long id)
        {
            Project project = unitOfWork.ProjectRepository.Find(id);
            return View(project);
           
        }

        //
        // POST: /Projects/Delete/5


        public ActionResult SearhResult()
        {
            TempData["PossibleProjects"] = TempData["PossibleProjects"];

            return View();
        }


        //
        // GET: /Projects/Edit/5

        //public PartialViewResult CreateStatus(long id)
        //{
        //    Project project = unitOfWork.ProjectRepository.Find(id);
        //    if (string.IsNullOrEmpty(project.allStatus))
        //    {
        //        project.allStatus = string.Empty;
        //    }
        //    return PartialView(project);
        //}

        //[HttpPost]
        //public PartialViewResult CreateStatus(Project project)
        //{
        //    Project projectOld = unitOfWork.ProjectRepository.Find(project.ProjectID);
        //    List<Task> taskList = new List<Task>();
        //    try
        //    {
        //        projectOld.allStatus = project.allStatus;
        //        unitOfWork.ProjectRepository.InsertOrUpdate(projectOld);
        //        unitOfWork.Save();
        //        taskList = unitOfWork.TaskRepository.ByProjectIncluding(project.ProjectID, task => task.Project).Include(task => task.Priority).Include(task => task.ChildTask).Include(task => task.Users).Include(task => task.Followers).Include(task => task.Labels).ToList();
        //        foreach (Task task in taskList)
        //        {
        //            task.Status = string.Empty;
        //            unitOfWork.Save();
        //        }
        //    }
        //    catch
        //    {
        //       // return View(projectOld);
        //    }
        //    //return RedirectToAction("Kanban", "Tasks", new { @ProjectID = project.ProjectID });
        //    return PartialView("_Kanban", taskList);
        //}

       

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            unitOfWork.ProjectRepository.Delete(id);
            unitOfWork.Save();
            return RedirectToAction("Index");

        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}