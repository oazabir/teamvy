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
    public class EmailSchedulersController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        //private PMToolContext context = new PMToolContext(); //It have to remove after code refactoring

        //
        // GET: /EmailSchedulers/

        public ViewResult Index()
        {
            //return View(context.EmailSchedulers.Include(emailscheduler => emailscheduler.Project).ToList());

            //return View(unitOfWork.EmailSchedulerRepository.in EmailSchedulers.Include(emailscheduler => emailscheduler.Project).ToList());

            return View(unitOfWork.EmailSchedulerRepository.GetEmailSchedulerAll());
        }

        //
        // GET: /EmailSchedulers/Details/5

        //public ViewResult Details(long id)
        //{
        //    EmailScheduler emailscheduler = context.EmailSchedulers.Single(x => x.SchedulerID == id);
        //    return View(emailscheduler);
        //}

        //
        // GET: /EmailSchedulers/Create

        public ActionResult Create()
        {
            ViewBag.PossibleProjects = unitOfWork.ProjectRepository.All; //context.Projects;
            EmailScheduler emailScheduler = new EmailScheduler();

            //IEnumerable<ScheduleType> scheduleTypes = Enum.GetValues(typeof(ScheduleType)).Cast<ScheduleType>();

            emailScheduler.SchedulerTitles = from item in unitOfWork.EmailSchedulerRepository.GetSchedulerList()
                                             select new SelectListItem
                                             {
                                                 Text = item.Value,
                                                 Value = item.Key
                                             };

            emailScheduler.ScheduleType = from item in unitOfWork.EmailSchedulerRepository.GetSchedulerTypeAll()
                                          select new SelectListItem
                                          {
                                              Text = item.Value,
                                              Value = item.Key
                                          };

            var recipientTypes = unitOfWork.EmailSchedulerRepository.GetRecipientUserTypeAll();
                //new Dictionary<string, string> {{ "0", "--- Select recipient users ---" }, 
                                                                //{ "1", "Task's Users" }, 
                                                                //{ "2", "Task's Followers" },
                                                                //{ "3", "Task's Users & Followers" },
                                                                // { "4", "Project's Users" }};

            emailScheduler.EmailRecipientUsers = from item in recipientTypes
                                                 select new SelectListItem
                                                 {
                                                     Text = item.Value,
                                                     Value = item.Key
                                                 };

            emailScheduler.Days = from item in unitOfWork.EmailSchedulerRepository.GetDaysOfWeek()
                                  select new SelectListItem
                                  {
                                      Text = item.Value,
                                      Value = item.Key
                                  };

            //IEnumerable<Week> days = Enum.GetValues(typeof(Week)).Cast<Week>();
            //emailScheduler.Days = from action in days
            //                      select new SelectListItem
            //                      {
            //                          Text = action.ToString(),
            //                          Value = ((int)action).ToString()
            //                      };

            //emailScheduler.ScheduledTime = new TimeSpan(11, 30, 0);

            //emailScheduler.ScheduledTime = new TimeSpan(15, 30, 0);

            emailScheduler.ScheduledDate = DateTime.Today;

            //String testDt = DateTime.Now.ToShortTimeString();

            //String testDt = DateTime.Now.ToString("hh:mm tt");

            //emailScheduler.ScheduledTime = Convert.ToDateTime(DateTime.Now.ToString("hh:mm tt"));

            emailScheduler.ScheduledTime = DateTime.Now.ToString("hh:mm tt");

            return View(emailScheduler);
        }

        //
        // POST: /EmailSchedulers/Create

        [HttpPost]
        public ActionResult Create(EmailScheduler emailscheduler)
        {
            emailscheduler.CreatedBy = (int)Membership.GetUser(WebSecurity.CurrentUserName).ProviderUserKey;
            emailscheduler.ModifiedBy = (int)Membership.GetUser(WebSecurity.CurrentUserName).ProviderUserKey;
            //timeLog.UserID = (int)Membership.GetUser(WebSecurity.CurrentUserName).ProviderUserKey;
            emailscheduler.CreateDate = DateTime.Now;
            emailscheduler.ModificationDate = DateTime.Now;
            emailscheduler.IsActive = true;

            //emailscheduler.ScheduledTime = new TimeSpan(10, 30, 0);

            if (ModelState.IsValid)
            {
                unitOfWork.EmailSchedulerRepository.InsertOrUpdate(emailscheduler);
                unitOfWork.Save();
                //return RedirectToAction("TaskTimeLog", new { @taskId = timeLog.TaskID, @sprintId = timeLog.SprintID });

                //context.EmailSchedulers.Add(emailscheduler);
                //context.SaveChanges();

                return RedirectToAction("Index");
            }

            ViewBag.PossibleProjects = unitOfWork.ProjectRepository.All; // context.Projects;
            //return View(emailscheduler);
            return RedirectToAction("Index");
        }

        //
        // GET: /EmailSchedulers/Edit/5

        //public ActionResult Edit(long id)
        //{
        //    EmailScheduler emailscheduler = context.EmailSchedulers.Single(x => x.SchedulerID == id);
        //    ViewBag.PossibleProjects = context.Projects;
        //    return View(emailscheduler);
        //}

        //
        // POST: /EmailSchedulers/Edit/5

        //[HttpPost]
        //public ActionResult Edit(EmailScheduler emailscheduler)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        context.Entry(emailscheduler).State = System.Data.Entity.EntityState.Modified;
        //        context.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.PossibleProjects = context.Projects;
        //    return View(emailscheduler);
        //}

        //
        // GET: /EmailSchedulers/Delete/5

        public ActionResult Delete(long id)
        {
            EmailScheduler emailscheduler = unitOfWork.EmailSchedulerRepository.Find(id);
            //context.EmailSchedulers.Single(x => x.SchedulerID == id);
            return View(emailscheduler);
        }

        //
        // POST: /EmailSchedulers/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            EmailScheduler emailscheduler = unitOfWork.EmailSchedulerRepository.Find(id);
            unitOfWork.EmailSchedulerRepository.Delete(id);
            unitOfWork.Save();
            //context.EmailSchedulers.Single(x => x.SchedulerID == id);
            //context.EmailSchedulers.Remove(emailscheduler);
            //context.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            //if (disposing) {
            //    context.Dispose();
            //}
            base.Dispose(disposing);
        }
    }
}