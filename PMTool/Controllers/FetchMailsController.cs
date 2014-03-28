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
            List<Mailer> mailerList = new List<Mailer>();
            List<Task> taskList = unitofWork.TaskRepository.AllIncludingForMail().ToList();
            List<UserProfile> userList = unitofWork.UserRepository.All();

            foreach (UserProfile user in userList)
            {
                // i++;
                string overdueTask = string.Empty;
                string todaysTask = string.Empty;
                string dueTommorrowTask = string.Empty;
                string futureTask = string.Empty;

                string messageBody = string.Empty;
                List<Task> userTaskList = taskList.Where(t => t.Users.Any(u => u.UserId == user.UserId)).ToList();

                if (userTaskList.Count() > 1)
                {
                    messageBody = "<b>Dear &nbsp;" + user.FirstName + "</b>,<br>" + "<b>You have the following tasks:</b><br>";
                    //overdueTask = "<ul>";
                    foreach (var task in userTaskList)
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
                    //overdueTask = "</ul>";


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