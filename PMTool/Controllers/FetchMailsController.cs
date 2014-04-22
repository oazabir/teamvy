using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PMTool.Models;
using PMTool.Repository;

namespace PMTool.Controllers
{
    public class FetchMailsController : ApiController
    {
        UnitOfWork unitofWork = new UnitOfWork();



        // GET api/FetchMails
        /// <summary>
        /// API to get email list
        /// Mahedee @20-03-14
        /// </summary>
        /// <returns></returns>
        public List<Mailer> Get()
        {
            return GenerateMailerList();
        }


        private List<Mailer> GenerateMailerList()
        {
            List<Mailer> mailerList = new List<Mailer>();
            List<Task> taskList = unitofWork.TaskRepository.AllIncludingForMail().ToList();
            List<UserProfile> userList = unitofWork.UserRepository.All();

            string styleTableHeader = "style= \"background-color:#0094ff; border:1px solid;\"";
            string styleGroupbyRow = "style= \"background-color:#57C0E1;\"";
            string styleTaskRow = "style= \"background-color:#A0D0FF;\"";

            foreach (UserProfile user in userList) //Generate mail for every user who have task
            {
                string overdueTask = string.Empty;
                string todaysTask = string.Empty;
                string dueTommorrowTask = string.Empty;
                string futureTask = string.Empty;

                //string messageHeader = "<table>";
                //string messageFooter = string.Empty;

                string messageBody = string.Empty;
 
                List<Task> userTaskList = taskList.Where(t => t.Users.Any(u => u.UserId == user.UserId)).ToList();

                if (userTaskList.Count() > 1)
                {
                    messageBody = "<b>Dear &nbsp;" + user.FirstName + "</b>,<br>" + "<b>Your assigned tasks are given below</b><br>";
                    messageBody += "<table><tr " + styleTableHeader + "><th>Task ID</th> <th>Task Title</th> <th>Start Date</th> <th>End Date</th> <th>Status</th></tr>";
                    //overdueTask = "<ul>";
                    foreach (var task in userTaskList)
                    {
                        if ((task.EndDate < DateTime.Today) && (task.ProjectStatusID != null && task.ProjectStatus.Name.ToLower() != "closed")) // overdue task
                        {
                            overdueTask += "<tr " + styleTaskRow + ">" + "<td>" + task.TaskUID + "</td>" + "<td>" + task.Title + "</td>" + "<td>" + (task.StartDate != null ? task.StartDate.Value.ToString("dd/MM/yyyy") : "") + "</td>" + "<td>" 
                                + (task.EndDate != null ? task.EndDate.Value.ToString("dd/MM/yyyy") : " ") + "</td>" + "<td>" + task.ProjectStatus.Name + "</td>" + "</tr>"; 
                                //"<li>" + task.Title + "</li>";
                        }
                        else if (task.StartDate == DateTime.Today && (task.ProjectStatusID != null && task.ProjectStatus.Name.ToLower() != "closed")) // todays task
                        {
                            todaysTask += "<tr " + styleTaskRow + ">" + "<td>" + task.TaskUID + "</td>" + "<td>" + task.Title + "</td>" + "<td>" + (task.StartDate != null ? task.StartDate.Value.ToString("dd/MM/yyyy") : "") + "</td>" + "<td>"
                                + (task.EndDate != null ? task.EndDate.Value.ToString("dd/MM/yyyy") : "") + "</td>" + "<td>" + task.ProjectStatus.Name + "</td>" + "</tr>"; 
                        }
                        else if (task.EndDate == DateTime.Today.AddDays(1) && (task.ProjectStatusID != null && task.ProjectStatus.Name.ToLower() != "closed")) //due tomorrow task
                        {
                            dueTommorrowTask += "<tr " + styleTaskRow + ">" + "<td>" + task.TaskUID + "</td>" + "<td>" + task.Title + "</td>" + "<td>" + (task.StartDate != null ? task.StartDate.Value.ToString("dd/MM/yyyy") : "") + "</td>" + "<td>"
                                + (task.EndDate != null ? task.EndDate.Value.ToString("dd/MM/yyyy") : "" )+ "</td>" + "<td>" + task.ProjectStatus.Name + "</td>" + "</tr>"; 
                        }
                        else if (task.StartDate > DateTime.Today && (task.ProjectStatusID == null || task.ProjectStatus.Name.ToLower() != "closed")) //Future task
                        {
                            futureTask += "<tr " + styleTaskRow + ">" + "<td>" + task.TaskUID + "</td>" + "<td>" + task.Title + "</td>" + "<td>" + (task.StartDate != null ? task.StartDate.Value.ToString("dd/MM/yyyy") : "") + "</td>" + "<td>"
                                + (task.EndDate != null ? task.EndDate.Value.ToString("dd/MM/yyyy") : "") + "</td>" + "<td>" + task.ProjectStatus.Name + "</td>" + "</tr>"; 
                        }
                    }
                    //overdueTask = "</ul>";


                    if (overdueTask != string.Empty)
                    {
                        messageBody += "<tr " + styleGroupbyRow + "><td colspan='5'>Overdue Task</td></tr>" + overdueTask;
                            //"<b>Overdue tasks:</b> <br>" + overdueTask;
                    }
                    if (todaysTask != string.Empty)
                    {
                        messageBody += "<tr " + styleGroupbyRow + "><td colspan='5'>Today's Task</td></tr>" + todaysTask;
                    }
                    if (dueTommorrowTask != string.Empty)
                    {
                        messageBody += "<tr " + styleGroupbyRow + "><td colspan='5'>Tomorrows Task</td></tr>" + dueTommorrowTask;
                    }
                    if (futureTask != string.Empty)
                    {
                        messageBody += "<tr " + styleGroupbyRow + "><td colspan='5'>Future Task</td></tr>" + futureTask;
                    }

                    messageBody += "</table></div><br /><div style='float:left;'><p>We sent you this email because you signed up in PMTool and tasks are assigned to you. <br /> Please don't reply this mail.</p><p>"
                        +"Regards,<br />PMTool</p></div>";

                    Mailer mailer = new Mailer();
                    mailer.UseMailID = user.UserName;
                    mailer.MailSubject = "Daily email digest: Task list from PM Tool";
                    mailer.HtmlMailBody = messageBody;
                    //mailer.HtmlMailBody = GenerateMailBody(user);
                    mailerList.Add(mailer);
                }
            }

            return mailerList;
        }

        private string GenerateMailBody(UserProfile user)
        {
            string overdueTask = string.Empty;
            string todaysTask = string.Empty;
            string dueTommorrowTask = string.Empty;
            string futureTask = string.Empty;
            string messageBody = string.Empty;
            IQueryable<Task> taskList = unitofWork.TaskRepository.GetTasksByUser(user);

            messageBody = "<b>Dear &nbsp;" + user.FirstName + "</b>,<br>" + "<b>You have the following tasks:</b><br>";
            foreach (Task task in taskList)
            {
                if (task.EndDate < DateTime.Today) // overdue task   
                {
                    overdueTask += "<li>" + task.Title + "</li>";
                }
                else if (task.StartDate == DateTime.Today) // todays task
                {
                    todaysTask += "<li>" + task.Title + "</li>";
                }
                else if (task.EndDate == DateTime.Today.AddDays(1)) //due tomorrow task
                {
                    dueTommorrowTask += "<li>" + task.Title + "</li>";
                }
                else if (task.StartDate > DateTime.Today) //Future task
                {
                    futureTask += "<li>" + task.Title + "</li>";
                }
            }

            if (overdueTask != string.Empty)
            {
                messageBody += "<b>Overdue tasks:</b> <br>" + overdueTask;
            }
            if (todaysTask != string.Empty)
            {
                messageBody += "<b>Today's tasks:</b> <br>" + todaysTask;
            }
            if (dueTommorrowTask != string.Empty)
            {
                messageBody += "<b>Due Tommorrow's tasks:</b> <br>" + dueTommorrowTask;
            }
            if (futureTask != string.Empty)
            {
                messageBody += "<b>Future tasks:</b> <br>" + futureTask;
            }

            return messageBody;
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}