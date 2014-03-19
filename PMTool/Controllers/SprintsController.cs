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

            return PartialView("~/Views/Tasks/_Kanban",taskList);
        }



        public ActionResult EditFromKanban(long id)
        {
            Sprint sprint = unitOfWork.SprintRepository.Find(id);

            return PartialView(sprint);
        }

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



        public ActionResult SprintBurnDown(long sprintId)
        {
            Sprint sprint = unitOfWork.SprintRepository.Find(sprintId);
            return View(sprint);
        }

        public JsonResult SprintBurnDownData(long sprintId)
        {
            Decimal sprintEstHour = 0;

            Sprint sprint = unitOfWork.SprintRepository.Find(sprintId);
            List<Task> sprintTasks = unitOfWork.TaskRepository.GetTasksBySprintID(sprintId);
            var chartList = new List<Chart>();

            sprintEstHour = sprintTasks.Sum(p => p.TaskHour);

            List<TimeLog> expendedTime = unitOfWork.TimeLogRepository.GetTimeLogBySprint(sprintId);

            //List<decimal> taskHour = sprintTasks.Sum(p => p.LoggedTime.Sum(q => q.TaskHour));
            var SprintTimeLog = from s in expendedTime
                            group s by new {s.EntryDate} into g
                            select new { g.Key.EntryDate, TotalHour = g.Sum(s => s.TaskHour) };

            int baseValue = 0;
            decimal remainingHour = sprintEstHour;

            for (DateTime dt = sprint.StartDate; dt <= sprint.EndDate && dt <= DateTime.Today; dt = dt.AddDays(1))
            {
                decimal entryHour = 0;
                entryHour = SprintTimeLog.Where(p => p.EntryDate == dt).Sum(q=> q.TotalHour);
                remainingHour = remainingHour - entryHour;
                Chart chart = new Chart { XValue = baseValue, YValue = remainingHour };
                chartList.Add(chart);
                baseValue++;
                
            }
            return Json(chartList, JsonRequestBehavior.AllowGet);
        }
    }
}
