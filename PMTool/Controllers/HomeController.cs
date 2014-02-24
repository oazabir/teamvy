using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using PMTool.Models;
using PMTool.Repository;

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
            List<Task> todaysTaskList = new List<Task>();
            todaysTaskList = unitOfWork.TaskRepository.GetTasksByUser(user).ToList();
                //.ByProjectIncluding(projectID, user, task => task.Project).Include(task => task.Priority).Include(task => task.ChildTask).Include(task => task.Users).Include(task => task.Followers).Include(task => task.Labels).ToList();
            ViewBag.TodaysTask = todaysTaskList;

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

    }
}
