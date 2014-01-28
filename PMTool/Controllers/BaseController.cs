using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using PMTool.Models;
using PMTool.Repository;

namespace PMTool.Controllers
{
    public class BaseController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        
        public BaseController()
        {
            if (!string.IsNullOrEmpty(WebSecurity.User.Identity.Name))
            {
                try
                {
                    User user = unitOfWork.UserRepository.GetUserByUserID((Guid)Membership.GetUser(WebSecurity.User.Identity.Name).ProviderUserKey);
                    LoadAssignedProjects(user);
                    LoadUnreadNotifications(user);
                    ViewBag.UserName = user.FirstName + " " + user.LastName;
                }
                catch
                {
                }
            }
        }

        public void LoadUnreadNotifications(User user)
        {
            List<Notification> notificationList = unitOfWork.NotificationRepository.UserUnreadNotification(user);
            ViewBag.Notifications = notificationList;
        }

        public void LoadAssignedProjects(User user)
        {
          List<Project> projectList= unitOfWork.ProjectRepository.GetAssignedProjectByUser(user);
          ViewBag.AssignedProjects = projectList;
        }

    }
}