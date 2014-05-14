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
using WebMatrix.WebData;

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
            return View(unitOfWork.ProjectRepository.AllbyUserIncluding((int)Membership.GetUser(WebSecurity.CurrentUserName).ProviderUserKey, project => project.Users).ToList());
        }


        public ViewResult OwnProjects()
        {
            TempData["Message"] = TempData["Message"];
            return View(unitOfWork.ProjectRepository.AllbyOwnerIncluding((int)Membership.GetUser(User.Identity.Name).ProviderUserKey, project => project.Users).ToList());
        }


        //
        // GET: /Projects/Details/5

        public ViewResult Details(long id)
        {
            Project project = unitOfWork.ProjectRepository.Find(id);
            long notificationID =Convert.ToInt64(Request.QueryString["notificationID"]);
            
            //unitOfWork.NotificationRepository.Delete(notificationID);
            //unitOfWork.Save();
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
                    UserProfile user = unitOfWork.UserRepository.GetUserByUserID((int)Membership.GetUser(WebSecurity.CurrentUserName).ProviderUserKey);
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

            /*To add Close status by default. Added by: Mahedee @06-03-14*/
            if (project.ProjectStatuses == null)
            {
                List<ProjectStatus> lstStatus = new List<ProjectStatus>();
                lstStatus.Add(new ProjectStatus { Name = "Closed", ProjectStatusID = 1});
                project.ProjectStatuses = lstStatus;
                project.allStatus = "Closed";
            }

            return View(project);
        }

        /// <summary>
        /// Get All possible user for assigning project
        /// </summary>
        /// <returns></returns>
        private List<SelectListItem> GetAllUser()
        {
            List<SelectListItem> allUsers = new List<SelectListItem>();
            List<UserProfile> userList = unitOfWork.UserRepository.All();
            foreach (UserProfile user in userList)
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
            project.CreatedBy = (int)Membership.GetUser().ProviderUserKey;
            project.ModifiedBy = (int)Membership.GetUser().ProviderUserKey;
            project.IsActive = true;

            if (ModelState.IsValid)
            {
                unitOfWork.ProjectRepository.InsertOrUpdate(project);
                AddAssignUser(project);
                AddProjectOwner(project);
                AddProjectStatus(project);
                project.allStatus = "";
                unitOfWork.Save();
                SaveNotification(project,true);
                
                return RedirectToAction("Index");
            }
            List<SelectListItem> allUsers = GetAllUser();
            ViewBag.PossibleUsers = allUsers;
            return View(project);
        }

        private void AddProjectStatus(Project project)
        {
            unitOfWork.ProjectStatusRepository.DeletebyProjectID(project.ProjectID);

            List<string> statuses = new List<string>();
            if (!string.IsNullOrEmpty(project.allStatus))
            {
                statuses = project.allStatus.Split(',').Distinct().ToList();
                foreach (string status in statuses)
                {
                    if (!string.IsNullOrEmpty(status) && IsSattusExistinroject(status, project))
                    {
                        ProjectStatus col = new ProjectStatus();
                        col.Name = status;
                        col.ProjectID = project.ProjectID;
                        unitOfWork.ProjectStatusRepository.InsertOrUpdate(col);
                    }
                }
            }
        }

        private bool IsSattusExistinroject(string status,Project project)
        {
            bool isvalid = false;
            ProjectStatus projectStatus = unitOfWork.ProjectStatusRepository.FindbyProjectIDAndProjectStatusName(project.ProjectID, status);
           if (status != null)
           {
               isvalid = true;
           }
           return isvalid;
        }

        private void SaveNotification(Project project,bool isProjectInsert)
        {
            if (project.Users != null)
            {
                foreach (UserProfile user in project.Users)
                {
                    

                    Notification notification = new Notification();
                    if (isProjectInsert)
                    {
                        UserProfile createdUser = unitOfWork.UserRepository.GetUserByUserID(project.CreatedBy);
                        notification.Title = createdUser.FirstName + " " + createdUser.LastName + " Has added you on the porject --" + project.Name;
                    }
                    else
                    {
                        UserProfile modifiedUser = unitOfWork.UserRepository.GetUserByUserID(project.ModifiedBy);
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
            project.Users = new List<UserProfile>();
            if (project.SelectedAssignedUsers != null)
            {
                foreach (string userID in project.SelectedAssignedUsers)
                {
                    UserProfile user = unitOfWork.UserRepository.GetUserByUserID(Convert.ToInt64(userID));
                    project.Users.Add(user);

                }
            }
        }

        /// <summary>
        /// Add Project Owner
        /// Mahedee @13-02-14
        /// </summary>
        /// <param name="project"></param>
        private void AddProjectOwner(Project project)
        {
            project.ProjectOwners = new List<UserProfile>();
            if (project.SelectedProjectsOwners != null)
            {
                foreach (string userID in project.SelectedProjectsOwners)
                {
                    UserProfile user = unitOfWork.UserRepository.GetUserByUserID(Convert.ToInt64(userID));
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
            project.ModifiedBy = (int)Membership.GetUser().ProviderUserKey;
            project.IsActive = true;
            List<UserProfile> userList = new List<UserProfile>();
            if (ModelState.IsValid)
            {
                AddAssignUser(project);
                AddProjectOwner(project);
                AddProjectStatus(project);
               userList= unitOfWork.ProjectRepository.InsertOrUpdate(project);
               project.allStatus = "";
                unitOfWork.Save();
                SaveNotification(project, false);
                string msg = "";
                foreach (UserProfile user in userList)
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
         
        [HttpPost]
        public ActionResult DeleteStatusByProject(long projectID,long statsusID)
        {
            string status = "";
            try
            {
                unitOfWork.ProjectStatusRepository.DeleteByProjectIDAndColID(statsusID, projectID);
                unitOfWork.Save();
                status = "success";
            }
            catch
            {
                status = "Error : One or more task is already attached with this status...";
            }
            return Content(status);

        }


        /// <summary>
        /// List of Sprint against project for burndown chart
        /// Mahedee @17-03-14
        /// </summary>
        /// <returns></returns>
        public ActionResult BurndownCharts()
        {
            List<Sprint> sprintList = unitOfWork.SprintRepository.AllByProjectID(1);
            ViewBag.SprintList = sprintList;
            return View();
        }


        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            Project project = unitOfWork.ProjectRepository.FindAllDependancyOfProject(id);
            if (project.Users.Count == 0 && project.ProjectOwners.Count == 0 && project.ProjectStatuses.Count == 0 && project.Sprints.Count == 0)
            {
                if (project.Tasks.Count == 0)
                {
                    unitOfWork.ProjectRepository.Delete(id);
                    List<Notification> notifications = unitOfWork.NotificationRepository.FindNotification(id);
                    notifications.ForEach(n => unitOfWork.NotificationRepository.Delete(n.NotificationID));
                    unitOfWork.Save();
                    return RedirectToAction("Index");
                }
                else { 
                    TempData["Message"] = "Delete project task";
                    return RedirectToAction("Index");
                }
            }
            else {
                TempData["Message"] = "Delete project users,owners,sprint and status";
                return RedirectToAction("Edit", new { id=id}); 
            }

        }


        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}