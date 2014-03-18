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
            var chart = new List<Chart>
                              {
                                  new Chart{XValue = 0, YValue = 20},
                                                                                                        
                                  new Chart{XValue = 1, YValue = 19},
                                  new Chart{XValue = 2, YValue = 17},
                                  new Chart{XValue = 3, YValue = 15},
                                  new Chart{XValue = 4, YValue = 12},
                                  new Chart{XValue = 5, YValue = 9},
                                  new Chart{XValue = 6, YValue = 8},
                                  new Chart{XValue = 7, YValue = 7},
                                  new Chart{XValue = 8, YValue = 6},
                                  new Chart{XValue = 9, YValue = 4},
                                  new Chart{XValue = 10, YValue = 0}
                              };

            return Json(chart, JsonRequestBehavior.AllowGet);
        }
    }
}
