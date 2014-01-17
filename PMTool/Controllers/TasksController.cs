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
    public class TasksController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();
        //
        // GET: /Tasks/

        public ViewResult Index()
        {
            return View(unitOfWork.TaskRepository.AllIncluding(task => task.Project).Include(task => task.Priority).Include(task => task.ChildTask).Include(task => task.Users).Include(task => task.Followers).Include(task => task.Labels).ToList());
        }

        //
        // GET: /Tasks/Details/5

        public ViewResult Details(long id)
        {
            Task task = unitOfWork.TaskRepository.Find(id);
            return View(task);
        }

        //
        // GET: /Tasks/Create

        public ActionResult Create()
        {
            List<SelectListItem> allUsers = GetAllUser();
            ViewBag.PossibleUsers = allUsers;

            ViewBag.PossibleProjects = unitOfWork.ProjectRepository.All;
            ViewBag.PossiblePriorities = unitOfWork.PriorityRepository.All;

            List<SelectListItem> allLabels = GetAllLabel();
            ViewBag.PossibleLabels = allLabels;
            return View();
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
            if (ModelState.IsValid)
            {
                unitOfWork.TaskRepository.InsertOrUpdate(task);
                AddAssignUser(task);
                AddFollower(task);
                AddLabel(task);

                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(task);
        }

        private void AddLabel(Task task)
        {
            task.Labels = new List<Label>();
            foreach (string labelID in task.SelectedLabels)
            {
                Label label = unitOfWork.LabelRepository.Find(Convert.ToInt64(labelID));
                task.Labels.Add(label);
            }
        }

        private void AddFollower(Task task)
        {
            task.Followers = new List<User>();
            foreach (string userID in task.SelectedFollowedUsers)
            {
                User user = unitOfWork.UserRepository.GetUserByUserID(new Guid(userID));
                task.Followers.Add(user);
            }
        }

        private void AddAssignUser(Task task)
        {
            task.Users = new List<User>();
            foreach (string userID in task.SelectedAssignedUsers)
            {
                User user = unitOfWork.UserRepository.GetUserByUserID(new Guid(userID));
                task.Users.Add(user);

            }
        }

        //
        // GET: /Tasks/Edit/5

        public ActionResult Edit(long id)
        {
            Task task = unitOfWork.TaskRepository.Find(id);
            ViewBag.PossibleProjects = unitOfWork.ProjectRepository.All;
            ViewBag.PossiblePriorities = unitOfWork.PriorityRepository.All;

            List<SelectListItem> allUsers = GetAllUser();
            ViewBag.PossibleUsers = allUsers;

            task.SelectedAssignedUsers = task.Users.Select(u => u.UserId.ToString()).ToList();
            task.SelectedFollowedUsers = task.Followers.Select(u => u.UserId.ToString()).ToList();
            task.SelectedLabels = task.Labels.Select(u => u.LabelID.ToString()).ToList();

            List<SelectListItem> allLabels = GetAllLabel();
            ViewBag.PossibleLabels = allLabels;
            return View(task);
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

        private List<SelectListItem> GetAllUser()
        {
            List<SelectListItem> allUsers = new List<SelectListItem>();
            List<User> userList = unitOfWork.UserRepository.All();
            foreach (User user in userList)
            {
                SelectListItem item = new SelectListItem { Value = user.UserId.ToString(), Text = user.FirstName + user.LastName };
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
                AddAssignUser(task);
                AddFollower(task);
                AddLabel(task);

                unitOfWork.TaskRepository.InsertOrUpdate(task);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }

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
            unitOfWork.TaskRepository.Delete(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}