using PMTool.Models;
using PMTool.Repository;
using PMTool.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace PMTool.Controllers
{
    public class ScheduleMailsAPIController : ApiController
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
            List<Mailer> lstTestMailer = new List<Mailer>();
            
            //Dictionary<int, string> lstOfEmailSchedules = unitofWork.EmailSchedulerRepository.GetSchedulerList();

            //foreach (KeyValuePair<int,string> item in lstOfEmailSchedules)
            //{
            //    if (item.Key == 1)  //Do for "Remainder email for provide estimation"
            //    {
            //        //Do for "Remainder email for provide estimation"

            //       lstTestMailer = EstimationMail();
            //    }
            //}

           
            //lstTestMailer.Add(new Mailer { UseMailID = "mahedee.hasan@gmail.com", HtmlMailBody = "This is a test mgs", MailSubject = "Hello msg" });

            List<EmailScheduler> emailschedulerlst = unitofWork.EmailSchedulerRepository.GetEmailSchedulerAll();

            foreach (var emailSchedule in emailschedulerlst) 
            {
                if (emailSchedule.SchedulerTitleID == 2) //Daily Status mail
                {

                }
            }



            lstTestMailer = EstimationMail();
            return lstTestMailer;
            //return new List<Mailer>();
        }


        /*
         Send a daily digest email to each user having tasks in projects, that are part of Sprints 
        and thus require "Remaining work hour" to be updated each day. It should be sent on a specific time set 
        at the project level. 
         */


        public List<Mailer> EstimationMail()
        {

            //check: if schedule type is daily
                //Get collection of time to sent email in a day

            //if(DateTime.Now >= 
            //for multiple time set check by foreach loop)
            
            //List<EmailSentStatus> lstEmailSentStatus = unitofWork.EmailSentStatusRepository.All.Where(p=>p.EmailSchedulerID == 1).ToList();
            //DateTime scheduleTime = unitofWork.EmailSchedulerRepository.All.Where(p => p.SchedulerID == 1 && lstEmailSentStatus.Any(sts => sts.ScheduleDateTime == DateTimeUtility.GetDateTime(DateTime.Today, p.ScheduledTime)));
            //unitofWork.EmailSchedulerRepository.All.Where(p => p.SchedulerID == 1 && lstEmailSentStatus.Any(sts => sts.ScheduleDateTime == DateTimeUtility.GetDateTime(DateTime.Today, p.ScheduledTime)));
            //if(unitofWork.EmailSentStatusRepository.EmailSentStatusBySchedulerId(1).Where(p=>p.ScheduleDateTime == DateTime.Now)
            

            //EmailScheduler objEmailScheduler = new EmailScheduler();

            //objEmailScheduler = unitofWork.EmailSchedulerRepository.All.Where(p=>p.SchedulerID == 1).ToList().FirstOrDefault;
            //if(unitofWork.EmailSentStatusRepository.EmailSentStatusBySchedulerId(1).Where(p=>p.ScheduleDateTime == 

            //return new List<Mailer>();

            List<Mailer> mailerList = new List<Mailer>();
            List<Task> taskList = unitofWork.TaskRepository.AllIncludingForMail().Where(t=>t.ProjectStatus.Name.ToLower() != "closed").ToList();
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

                //List<Task> userTaskList = taskList.Where(t => t.Users.Any(u => u.UserId == user.UserId) && t.ProjectStatus.Name.ToLower() != "closed").ToList();
                List<Task> userTaskList = taskList.Where(t => t.Users.Any(u => u.UserId == user.UserId)).ToList();


                if (userTaskList.Count() > 1)
                {
                    messageBody = "<b>Dear &nbsp;" + user.FirstName + "</b>,<br>" + "<b>Your assigned tasks with estimated remaining hours are given below</b><br>";
                    messageBody += "<table><tr " + styleTableHeader + "><th>Entry Date</th> <th>Effort Hours</th> <th>Remaining Hours</th> <th>Users</th></tr>";
                    //overdueTask = "<ul>";
                    foreach (var task in userTaskList)
                    {
                        //List<TimeLog> taskTimeLog = new List<TimeLog>();
                        var taskTimeLog = unitofWork.TimeLogRepository.GetTimeLogByTaskId(task.TaskID);

                        messageBody += "<tr " + styleGroupbyRow + "><td colspan='4'> Task: "+ task.Title + "</td></tr>";
                        foreach (var timeLog in taskTimeLog)
                        {
                            messageBody += "<tr " + styleTaskRow + ">" + "<td>" + (timeLog.EntryDate != null ? timeLog.EntryDate.Value.ToString("dd/MM/yyyy") : "") + "</td>" + "<td>" + timeLog.TaskHour + "</td>" + "<td>" + timeLog.RemainingHour+ "</td>" + "<td>"
                              + timeLog.User.FirstName + " " + timeLog.User.LastName + "</td>"+ "</tr>";
                        }

                        if (taskTimeLog == null || taskTimeLog.Count() <= 0)
                        {
                            messageBody += "<tr " + styleTaskRow + ">" + "<td>" + "N/A" + "</td>" + "<td>" + "No Effort" + "</td>" + "<td>" + task.TaskHour + "</td>" + "<td>"
                              + "" + "N/A" + "" + "</td>" + "</tr>";
                        }
                        

                    }
                    //overdueTask = "</ul>";


                    //if (overdueTask != string.Empty)
                    //{
                    //    messageBody += "<tr " + styleGroupbyRow + "><td colspan='5'>Overdue Task</td></tr>" + overdueTask;
                    //    //"<b>Overdue tasks:</b> <br>" + overdueTask;
                    //}
                    //if (todaysTask != string.Empty)
                    //{
                    //    messageBody += "<tr " + styleGroupbyRow + "><td colspan='5'>Today's Task</td></tr>" + todaysTask;
                    //}
                    //if (dueTommorrowTask != string.Empty)
                    //{
                    //    messageBody += "<tr " + styleGroupbyRow + "><td colspan='5'>Tomorrows Task</td></tr>" + dueTommorrowTask;
                    //}
                    //if (futureTask != string.Empty)
                    //{
                    //    messageBody += "<tr " + styleGroupbyRow + "><td colspan='5'>Future Task</td></tr>" + futureTask;
                    //}

                    messageBody += "</table></div><br /><div style='float:left;'><p>We sent you this email because you signed up in PMTool and tasks are assigned to you. <br /> Please don't reply this mail.</p><p>"
                        + "Regards,<br />PMTool</p></div>";

                    Mailer mailer = new Mailer();
                    mailer.UseMailID = user.UserName;
                    mailer.MailSubject = "Remainder mail of task estimation";
                    mailer.HtmlMailBody = messageBody;
                    //mailer.HtmlMailBody = GenerateMailBody(user);
                    mailerList.Add(mailer);
                }
            }

            return mailerList;


        }



        



    }
}
