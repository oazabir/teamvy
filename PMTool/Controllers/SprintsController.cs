using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PMTool.Models;
using PMTool.Repository;

namespace PMTool.Controllers
{
    public class SprintsController : Controller
    {
        //
        // GET: /Sprints/
        UnitOfWork unitOfWork = new UnitOfWork();
        
        public ActionResult CreateSprintFromKanban(long id)
        {
            Sprint sprint = new Sprint();
            sprint.StartDate = DateTime.Now;
            sprint.EndDate = DateTime.Now;     
            sprint.ProjectID = id;
            sprint.Project = unitOfWork.ProjectRepository.Find(sprint.ProjectID);

            return PartialView(sprint); 
        }


        [HttpPost]
        public PartialViewResult CreateSprintFromKanban(Sprint sprint)
        {

            if (ModelState.IsValid)
            {
                unitOfWork.SprintRepository.InsertOrUpdate(sprint);
                unitOfWork.Save();
            }

            Project project = unitOfWork.ProjectRepository.Find(sprint.ProjectID);
            ViewBag.CurrentProject = project;
            List<Task> taskList = new List<Task>();
            taskList = unitOfWork.TaskRepository.GetTasksBySprintID(sprint.SprintID);

            return PartialView("~/Views/Tasks/_Kanban", taskList);
        }

        public ActionResult CreateSprint(long projectId)
        {
            Sprint sprint = new Sprint();
            sprint.StartDate = DateTime.Now;
            sprint.EndDate = DateTime.Now;
            sprint.ProjectID = projectId;
            sprint.Project = unitOfWork.ProjectRepository.Find(sprint.ProjectID);

            return View(sprint);
        }

        [HttpPost]
        public ActionResult CreateSprint(Sprint sprint)
        {

            if (ModelState.IsValid)
            {
                unitOfWork.SprintRepository.InsertOrUpdate(sprint);
                unitOfWork.Save();
                return RedirectToAction("Sprint", new { @projectId = sprint.ProjectID });
            }

            Project project = unitOfWork.ProjectRepository.Find(sprint.ProjectID);
            ViewBag.CurrentProject = project;
            List<Task> taskList = new List<Task>();
            taskList = unitOfWork.TaskRepository.GetTasksBySprintID(sprint.SprintID);

            return RedirectToAction("Sprint", new { @projectId = sprint.ProjectID });

            //return PartialView();
        }
/// <summary>
/// The following view Result Method is not use right now
/// </summary>
/// <param name="SelectedProjectID"></param>
/// <returns></returns>
        public ViewResult Sprint(string SelectedProjectID)
        {
            List<Sprint> lstSprint = new List<Sprint>();
            long projectId = Convert.ToInt32(SelectedProjectID);
            lstSprint = unitOfWork.SprintRepository.All.Where(p => p.ProjectID == projectId).ToList();
            return View(lstSprint);
           
        }

        public ViewResult Index()
        {
            //return View(unitOfWork.SprintRepository.All);
            List<Project> lstProject = new List<Project>();
            lstProject = unitOfWork.ProjectRepository.All.ToList();
            ViewBag.AllProjects = lstProject;
            ViewBag.AllSprints = null;
            return View();
        }

        /// <summary>
        /// For Displaying the List of Sprint in index page the below Method is used
        /// </summary>
        /// <param name="SelectedProjectID"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(string SelectedProjectID)
        {
            List<Sprint> lstSprint = new List<Sprint>();
            List<Project> lstProject = new List<Project>();

            if (SelectedProjectID != "")
            {
                long projectId = Convert.ToInt32(SelectedProjectID);
                lstSprint = unitOfWork.SprintRepository.All.Where(p => p.ProjectID == projectId).ToList();
                lstProject = unitOfWork.ProjectRepository.All.ToList();
                ViewBag.AllProjects = lstProject;
                ViewBag.AllSprints = lstSprint;
                return View();
            }

            else 
            {
                //lstSprint = unitOfWork.SprintRepository.All.Where(p => p.ProjectID == projectId).ToList();
                lstProject = unitOfWork.ProjectRepository.All.ToList();
                ViewBag.AllProjects = lstProject;
                ViewBag.AllSprints = lstSprint;
                ViewBag.Flag = "1";
                return View();
            }
            
        }

    

        [HttpPost]
        public ActionResult Delete(long sprintId)
        {
            
            string status = "";
            try
            {
                unitOfWork.SprintRepository.Delete(sprintId);
                unitOfWork.Save();
                status = "success";
            }
            catch
            {
                status = "Error : One or more task is already attached in this sprint...";
            }
            return Content(status);

        }




        /// <summary>
        /// Edit sprint from kanban
        /// Added by mahedee
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult EditFromKanban(long id)
        {
            Sprint sprint = unitOfWork.SprintRepository.Find(id);

            return PartialView(sprint);
        }


        /// <summary>
        /// Edit sprint from kanban
        /// Added by mahedee for post data
        /// </summary>
        /// <param name="sprint"></param>
        /// <returns></returns>
        [HttpPost]
        public PartialViewResult EditFromKanban(Sprint sprint)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.SprintRepository.InsertOrUpdate(sprint);
                unitOfWork.Save();
            }

            Project project = unitOfWork.ProjectRepository.Find(sprint.ProjectID);
            ViewBag.CurrentProject = project;
            List<Task> taskList = new List<Task>();
            taskList = unitOfWork.TaskRepository.GetTasksBySprintID(sprint.SprintID);

            return PartialView("_Kanban", taskList);
        }


        /// <summary>
        /// Added by Mahedee @19-03-14
        /// </summary>
        /// <param name="sprintId"></param>
        /// <returns></returns>
        public ActionResult SprintBurnDown(long sprintId)
        {
            Sprint sprint = unitOfWork.SprintRepository.Find(sprintId);
            return View(sprint);
        }

        /// <summary>
        /// Get sprint burndown data against sprint id
        /// Added by Mahedee @19-03-14
        /// </summary>
        /// <param name="sprintId"></param>
        /// <returns></returns>
        public JsonResult SprintBurnDownData(long sprintId)
        {
            Decimal sprintEstHour = 0;

            Sprint sprint = unitOfWork.SprintRepository.Find(sprintId);
            List<Task> sprintTasks = unitOfWork.TaskRepository.GetTasksBySprintID(sprintId);
            var chartList = new List<Chart>();

            sprintEstHour = sprintTasks.Sum(p => p.TaskHour);

            List<TimeLog> expendedTime = unitOfWork.TimeLogRepository.GetTimeLogBySprint(sprintId);

            var SprintTimeLog = from s in expendedTime
                            group s by new {s.EntryDate} into g
                            //select new { g.Key.EntryDate, TotalHour = g.Sum(s => s.TaskHour) };
                            select new { g.Key.EntryDate, TotalRemaingHour = g.Sum(s => s.RemainingHour) };


            int baseValue = 0;
            decimal remainingHour = sprintEstHour;

            for (DateTime dt = sprint.StartDate; dt <= sprint.EndDate && dt <= DateTime.Today; dt = dt.AddDays(1))
            {
                //decimal entryHour = 0;
                //entryHour = SprintTimeLog.Where(p => p.EntryDate == dt).Sum(q=> q.TotalHour);
                decimal? totalRemainingHour = 0;
                totalRemainingHour = SprintTimeLog.Where(p => p.EntryDate == dt).Sum(q => q.TotalRemaingHour);

                //remainingHour = remainingHour - entryHour;
                //List<TimeLog> lstTask = unitOfWork.TimeLogRepository.All.Where(t=>t.SprintID == sprintId
             
                Chart chart = new Chart { XValue = baseValue, YValue = totalRemainingHour};
                chartList.Add(chart);
                baseValue++;
                
            }
            return Json(chartList, JsonRequestBehavior.AllowGet);
        }


        public JsonResult SprintBurnDownEstimetedData(long sprintId)
        {
            Decimal sprintEstHour = 0;

            Sprint sprint = unitOfWork.SprintRepository.Find(sprintId);
            List<Task> sprintTasks = unitOfWork.TaskRepository.GetTasksBySprintID(sprintId);
            var chartList = new List<Chart>();

            sprintEstHour = sprintTasks.Sum(p => p.TaskHour);

            int totalDay = (int)(sprint.EndDate - sprint.StartDate).TotalDays;

            decimal y = sprintEstHour;
            for (int x = 0; x < totalDay; x++)
            {
                Chart chart = new Chart { XValue = x, YValue = y };
                chartList.Add(chart);

                y = y - (sprintEstHour / totalDay);
            }

            return Json(chartList, JsonRequestBehavior.AllowGet);
        }

        


    }
}
