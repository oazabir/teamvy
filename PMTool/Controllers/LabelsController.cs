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
    [Authorize]
    public class LabelsController : BaseController
    {
        private UnitOfWork uitofWork = new UnitOfWork();

        //
        // GET: /Labels/

        public ViewResult Index()
        {
            return View(uitofWork.LabelRepository.AllIncluding(label => label.Tasks).ToList());
        }

        //
        // GET: /Labels/Details/5

        public ViewResult Details(long id)
        {
            Label label = uitofWork.LabelRepository.Find(id);
            return View(label);
        }

        //
        // GET: /Labels/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Labels/Create

        [HttpPost]
        public ActionResult Create(Label label)
        {
            label.ModificationDate = DateTime.Now;
            label.CreateDate = DateTime.Now;
            label.ActionDate = DateTime.Now;
            label.CreatedBy = (Guid)Membership.GetUser().ProviderUserKey;
            label.ModifieddBy = (Guid)Membership.GetUser().ProviderUserKey;
            if (ModelState.IsValid)
            {
                uitofWork.LabelRepository.InsertOrUpdate(label);
                uitofWork.Save();
                return RedirectToAction("Index");  
            }

            return View(label);
        }
        
        //
        // GET: /Labels/Edit/5
 
        public ActionResult Edit(long id)
        {
            Label label = uitofWork.LabelRepository.Find(id);
            return View(label);
        }

        //
        // POST: /Labels/Edit/5

        [HttpPost]
        public ActionResult Edit(Label label)
        {
            label.ModificationDate = DateTime.Now;
            label.ActionDate = DateTime.Now;
            label.ModifieddBy = (Guid)Membership.GetUser().ProviderUserKey;
            if (ModelState.IsValid)
            {
                uitofWork.LabelRepository.InsertOrUpdate(label);
                uitofWork.Save();
                return RedirectToAction("Index");
            }
            return View(label);
        }

        //
        // GET: /Labels/Delete/5
 
        public ActionResult Delete(long id)
        {
            Label label = uitofWork.LabelRepository.Find(id);
            return View(label);
        }

        //
        // POST: /Labels/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            uitofWork.LabelRepository.Delete(id);
            uitofWork.Save();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}