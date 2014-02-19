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
        public ActionResult CreateFromKanban(long projectID)
        {
            Sprint sprint = new Sprint();
            sprint.StartDate = DateTime.Now;
            sprint.EndDate = DateTime.Now;
            sprint.ProjectID = projectID;

            return PartialView(); 
        } 

        [HttpPost]
        public PartialViewResult CreateFromKanban(Sprint sprint)
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
    }
}
