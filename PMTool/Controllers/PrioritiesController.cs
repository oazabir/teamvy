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
    public class PrioritiesController : BaseController
    {
        private PMToolContext context = new PMToolContext();

        //
        // GET: /Priorities/

        public ViewResult Index()
        {
            return View(context.Priorities.ToList());
        }

        //
        // GET: /Priorities/Details/5

        public ViewResult Details(int id)
        {
            Priority priority = context.Priorities.Single(x => x.PriorityID == id);
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
                context.Priorities.Add(priority);
                context.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(priority);
        }
        
        //
        // GET: /Priorities/Edit/5
 
        public ActionResult Edit(int id)
        {
            Priority priority = context.Priorities.Single(x => x.PriorityID == id);
            return View(priority);
        }

        //
        // POST: /Priorities/Edit/5

        [HttpPost]
        public ActionResult Edit(Priority priority)
        {
            if (ModelState.IsValid)
            {
                context.Entry(priority).State = System.Data.Entity.EntityState.Modified;
                context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(priority);
        }

        //
        // GET: /Priorities/Delete/5
 
        public ActionResult Delete(int id)
        {
            Priority priority = context.Priorities.Single(x => x.PriorityID == id);
            return View(priority);
        }

        //
        // POST: /Priorities/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Priority priority = context.Priorities.Single(x => x.PriorityID == id);
            context.Priorities.Remove(priority);
            context.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}