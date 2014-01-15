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
    public class ProjectsController : Controller
    {
        //private PMToolContext context = new PMToolContext();
        private UnitOfWork unitOfWork = new UnitOfWork();
        //
        // GET: /Projects/

        public ViewResult Index()
        {

            return View(unitOfWork.ProjectRepository.AllIncluding(project => project.Users).ToList());
        }

        //
        // GET: /Projects/Details/5

        public ViewResult Details(long id)
        {
            Project project = unitOfWork.ProjectRepository.Find(id);
            return View(project);
        }

        //
        // GET: /Projects/Create

        public ActionResult Create()
        {
            return View();
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
                unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(project);
        }

        //
        // GET: /Projects/Edit/5

        public ActionResult Edit(long id)
        {
            Project project = unitOfWork.ProjectRepository.Find(id);
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
            if (ModelState.IsValid)
            {
                unitOfWork.ProjectRepository.InsertOrUpdate(project);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
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

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            unitOfWork.ProjectRepository.Delete(id);
            unitOfWork.Save();
            return RedirectToAction("Index");

        }

        protected override void Dispose(bool disposing)
        {
            //if (disposing)
            //{
            //    context.Dispose();
            //}
            base.Dispose(disposing);
        }
    }
}