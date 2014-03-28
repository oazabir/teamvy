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
using WebMatrix.WebData;

namespace PMTool.Controllers
{
    public class BaseController : Controller
    { 
        private UnitOfWork unitOfWork = new UnitOfWork();
          
        
        public BaseController()
        {
            if (!string.IsNullOrEmpty(WebSecurity.CurrentUserName))
            {
                try
                {
                    UserProfile user = unitOfWork.UserRepository.GetUserByUserID((int)Membership.GetUser(WebSecurity.CurrentUserName).ProviderUserKey);
                    LoadAssignedProjects(user);
                    LoadUnreadNotifications(user);
                    ViewBag.UserName = user.FirstName + " " + user.LastName;
                }
                catch
                {
                }
            }
        }

        public void LoadUnreadNotifications(UserProfile user)
        {

        }

        public void LoadAssignedProjects(UserProfile user)
        {
          List<Project> projectList= unitOfWork.ProjectRepository.GetAssignedProjectByUser(user);
          ViewBag.AssignedProjects = projectList;
        }

    }
}