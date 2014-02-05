using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMTool.Models;
using PMTool.Repository;

namespace PMTool.Controllers
{
    [Authorize]
    public class PrioritiesController : BaseController
    {
        private UnitOfWork unitofWork = new UnitOfWork();
        //
        // GET: /Priorities/

        public ViewResult Index()
        {
            return View(unitofWork.PriorityRepository.All);
        }

        //
        // GET: /Priorities/Details/5

        public ViewResult Details(int id)
        {
            Priority priority = unitofWork.PriorityRepository.Find(id);
            return View(priority);
        }

        //
        // GET: /Priorities/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Priorities/Create

        [HttpPost]
        public ActionResult Create(Priority priority)
        {
            if (ModelState.IsValid)
            {
                unitofWork.PriorityRepository.InsertOrUpdate(priority);
                unitofWork.Save();
                return RedirectToAction("Index");  
            }

            return View(priority);
        }
        
        //
        // GET: /Priorities/Edit/5
 
        public ActionResult Edit(int id)
        {
            Priority priority = unitofWork.PriorityRepository.Find(id);
            return View(priority);
        }

        //
        // POST: /Priorities/Edit/5

        [HttpPost]
        public ActionResult Edit(Priority priority)
        {
            if (ModelState.IsValid)
            {
                unitofWork.PriorityRepository.InsertOrUpdate(priority);
                unitofWork.Save();
                return RedirectToAction("Index");
            }
            return View(priority);
        }

        //
        // GET: /Priorities/Delete/5
 
        public ActionResult Delete(int id)
        {
            Priority priority = unitofWork.PriorityRepository.Find(id);
            return View(priority);
        }

        //
        // POST: /Priorities/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            unitofWork.PriorityRepository.Delete(id);
            unitofWork.Save();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}