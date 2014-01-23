using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMTool.Models;
using PMTool.Repository;

namespace PMTool.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

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

        public ActionResult Search(Search search)
        {
            List<Project> projectList = new List<Project>();
            using (UnitOfWork unitOfWork = new UnitOfWork())
            {
                if (!string.IsNullOrEmpty(search.SearchParam))
                projectList = unitOfWork.ProjectRepository.GetListbyName(search.SearchParam);
            }
            TempData["PossibleProjects"] = projectList;
            return RedirectToAction("SearhResult","Projects");
        }
    }
}
