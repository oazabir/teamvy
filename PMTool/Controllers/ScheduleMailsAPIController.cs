using PMTool.Models;
using PMTool.Repository;
using PMTool.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            List<Mailer> objOflstMailer = new List<Mailer>();
            
            //Dictionary<int, string> lstOfEmailSchedules = unitofWork.EmailSchedulerRepository.GetSchedulerList();

            //foreach (KeyValuePair<int,string> item in lstOfEmailSchedules)
            //{
            //    if (item.Key == 1)  //Do for "Remainder email for provide estimation"
            //    {
            //        //Do for "Remainder email for provide estimation"

            //       objOflstMailer = EstimationMail();
            //    }
            //}

           
            //objOflstMailer.Add(new Mailer { UseMailID = "mahedee.hasan@gmail.com", HtmlMailBody = "This is a test mgs", MailSubject = "Hello msg" });

            List<EmailScheduler> emailschedulerlst = unitofWork.EmailSchedulerRepository.GetEmailSchedulerAll(); // This Method Return All the Schedule
            
            DateTime cdate = DateTime.Now;
            DateTime onlyDate = cdate.Date;
            TimeSpan ts1 = cdate.TimeOfDay;
            DateTime currentdateTime = onlyDate + ts1;

            List<EmailSentStatus> objOfEmailSent = new List<EmailSentStatus>();
            objOfEmailSent = unitofWork.EmailSentStatusRepository.GetEmailSentStatuseAll();


            foreach (var emailSchedule in emailschedulerlst) 
            {
                if (emailSchedule.SchedulerTitleID == 2) //This is for Daily Status mail
                {
                        //List<EmailSentStatus> objOfEmailSent = new List<EmailSentStatus>();
                        //objOfEmailSent = unitofWork.EmailSentStatusRepository.GetEmailSentStatuseAll();
                        
                        //DateTime cdate = DateTime.Now;
                        //DateTime onlyDate = cdate.Date;
                        //TimeSpan ts1 = cdate.TimeOfDay;
                        //DateTime currentdateTime = onlyDate + ts1;
                        
                        DateTime scheduleDateTime = DateTime.ParseExact(emailSchedule.ScheduledTime, "h:mm tt", CultureInfo.InvariantCulture);

                        if (objOfEmailSent.Count>0)
                        {
                            foreach (var ObjofList in objOfEmailSent)
                            {

                                bool emailStatus = unitofWork.EmailSentStatusRepository.EmailSentStatus(ObjofList.EmailSchedulerID, ObjofList.ScheduleTypeID, ObjofList.ScheduleDateTime);
                                if (emailStatus = true && currentdateTime >= scheduleDateTime)
                                {
                                    continue;
                                }

                                else
                                {
                                    objOflstMailer = DailyTaskStatus(emailSchedule);
                                    objOflstMailer.AddRange(DailyTaskStatusForCreator());

                                    if (objOflstMailer.Count() > 0)
                                    {

                                        EmailSentStatus objEmailSentStatus = new EmailSentStatus();
                                        objEmailSentStatus.EmailSchedulerID = 2;
                                        objEmailSentStatus.ScheduleTypeID = 1;
                                        DateTime date = DateTime.ParseExact(emailSchedule.ScheduledTime, "h:mm tt", CultureInfo.InvariantCulture);
                                        TimeSpan ts = date.TimeOfDay;
                                        objEmailSentStatus.ScheduleDateTime = DateTime.Today.Add(ts);
                                        objEmailSentStatus.SentDateTime = DateTime.Now;
                                        objEmailSentStatus.SentStatus = true;
                                        objEmailSentStatus.ActionTime = DateTime.Now;

                                        unitofWork.EmailSentStatusRepository.InsertOrUpdate(objEmailSentStatus);
                                        unitofWork.Save();

                                    }
                                }
                            }
                        }

                        else 
                        {
                            objOflstMailer = DailyTaskStatus(emailSchedule);

                            if (objOflstMailer.Count() > 0)
                            {

                                EmailSentStatus objEmailSentStatus = new EmailSentStatus();
                                objEmailSentStatus.EmailSchedulerID = 2;
                                objEmailSentStatus.ScheduleTypeID = 1;
                                DateTime date = DateTime.ParseExact(emailSchedule.ScheduledTime, "h:mm tt", CultureInfo.InvariantCulture);
                                TimeSpan ts = date.TimeOfDay;
                                objEmailSentStatus.ScheduleDateTime = DateTime.Today.Add(ts);
                                objEmailSentStatus.SentDateTime = DateTime.Now;
                                objEmailSentStatus.SentStatus = true;
                                objEmailSentStatus.ActionTime = DateTime.Now;

                                unitofWork.EmailSentStatusRepository.InsertOrUpdate(objEmailSentStatus);
                                unitofWork.Save();

                            }
                        }
                           
                }

                else if (emailSchedule.SchedulerTitleID == 4) // This is for Notification Email
                {
                    DateTime scheduleDateTime = DateTime.ParseExact(emailSchedule.ScheduledTime, "h:mm tt", CultureInfo.InvariantCulture);
                    List<Notification> ListOfNotification = unitofWork.NotificationRepository.GetNotificationDetails();              
  
                    if (objOfEmailSent.Count > 0)
                    {
                        foreach (var objOfnotification in ListOfNotification)
                        {
                            bool NotificationDet = unitofWork.NotificationRepository.GetNotificationDet(objOfnotification.ProjectID);
                            if (NotificationDet == true && objOfnotification.ActionDate > scheduleDateTime && objOfnotification.ActionDate < scheduleDateTime)
                            {
                                //objOflstMailer = NotificationEmail();
                            }
                        }
                    }
                    else 
                    {
                        // 
                    }

                  
                }


            }

            return objOflstMailer;
            //return new List<Mailer>();
        }

        /*
         Send a daily Task Status email to each user having tasks in projects, that are part of Sprints.
         It should be sent on a specific time setat the project level. 
         */
        public List<Mailer> DailyTaskStatus(EmailScheduler objEmailScheduler ) 
        {
            List<Mailer> mailerList = new List<Mailer>();
            List<Task> taskList = unitofWork.TaskRepository.AllIncludingForMail().Where(t => t.ProjectStatus.Name.ToLower() != "closed").ToList();
            List<UserProfile> userList = unitofWork.UserRepository.All();

            string styleTableHeader = "style= \"background-color:#0094ff; border:1px solid;\"";
            string styleGroupbyRow = "style= \"background-color:#57C0E1;\"";
            string styleTaskRow = "style= \"background-color:#A0D0FF;\"";

            foreach(UserProfile objOfuser in userList)
            {
                string messageBody = string.Empty;
                List<Task> userTaskList = taskList.Where(a => a.Users.Any(b => b.UserId == objOfuser.UserId) && a.ProjectID == objEmailScheduler.ProjectID).ToList();

                if (userTaskList.Count > 0)
                {
                    messageBody = "<b>Dear &nbsp;" + objOfuser.FirstName + "</b>,<br>" + "<b>Your Daily Tasks Status are given below</b><br>";
                    messageBody += "<table><tr " + styleTableHeader + "><th>Task ID</th> <th>Task Title</th> <th>Task Status</th> <th>Project</th></tr>";

                    foreach (var task in userTaskList)
                    {                 
                        messageBody += "<tr> ";
                        messageBody += "<td " + styleTaskRow +    "> "+ task.TaskUID + "</td>";
                        messageBody += "<td " + styleGroupbyRow + "> " + task.Title + "</td>";
                        //string val = "";
                        //if(task.ProjectStatus.Name == "")
                          //  val = task.ProjectStatus.Name;
                        messageBody += "<td " + styleGroupbyRow + "> " + ((task.ProjectStatus==null) ? " " : task.ProjectStatus.Name )+ "</td>";
                        messageBody += "<td " + styleGroupbyRow + "> " + task.Project.Name + "</td>";
                        messageBody += "</tr>";
                    }

                    messageBody += "</table></div><br /><div style='float:left;'><p>We sent you this email because you signed up in PMTool and tasks are assigned to you. <br /> Please don't reply To this mail.</p><p>"
                       + "Regards,<br />PMTool</p></div>";

                    Mailer mailer = new Mailer();
                    mailer.UseMailID = objOfuser.UserName;
                    mailer.MailSubject = "Daily Task Status";
                    mailer.HtmlMailBody = messageBody;
                    mailerList.Add(mailer);
                }
            }

            return mailerList;
        }


        /*
 Send a daily Task Status email to each user having tasks in projects, that are part of Sprints.
 It should be sent on a specific time setat the project level. 
 */
        public List<Mailer> DailyTaskStatusForCreator()
        {
            List<Mailer> mailerList = new List<Mailer>();
            List<Task> taskList = unitofWork.TaskRepository.AllIncludingForMail().Where(t => t.ProjectStatus.Name.ToLower() != "closed").ToList();
            List<UserProfile> userList = unitofWork.UserRepository.All();

            string styleTableHeader = "style= \"background-color:#0094ff; border:1px solid;\"";
            string styleGroupbyRow = "style= \"background-color:#57C0E1;\"";
            string styleTaskRow = "style= \"background-color:#A0D0FF;\"";

            foreach (UserProfile objOfuser in userList)
            {
                string messageBody = string.Empty;
                List<Task> userTaskList = taskList.Where(a => a.CreatedByUser == objOfuser).ToList();

                if (userTaskList.Count > 0)
                {
                    messageBody = "<b>Dear &nbsp;" + objOfuser.FirstName + "</b>,<br>" + "<b>Your Daily Tasks Status are given below</b><br>";
                    messageBody += "<table><tr " + styleTableHeader + "><th>Task ID</th> <th>Task Title</th> <th>Task Status</th> <th>Project</th></tr>";

                    foreach (var task in userTaskList)
                    {
                        messageBody += "<tr> ";
                        messageBody += "<td " + styleTaskRow + "> " + task.TaskUID + "</td>";
                        messageBody += "<td " + styleGroupbyRow + "> " + task.Title + "</td>";
                        string val = "";
                        //if(task.ProjectStatus.Name == "")
                        //  val = task.ProjectStatus.Name;
                        messageBody += "<td " + styleGroupbyRow + "> " + ((task.ProjectStatus == null) ? " " : task.ProjectStatus.Name) + "</td>";
                        messageBody += "<td " + styleGroupbyRow + "> " + task.Project.Name + "</td>";
                        messageBody += "</tr>";
                    }

                    messageBody += "</table></div><br /><div style='float:left;'><p>We sent you this email because you signed up in PMTool and tasks are assigned to you. <br /> Please don't reply this mail.</p><p>"
                       + "Regards,<br />PMTool</p></div>";

                    Mailer mailer = new Mailer();
                    mailer.UseMailID = objOfuser.UserName;
                    mailer.MailSubject = "Daily Task Status";
                    mailer.HtmlMailBody = messageBody;
                    mailerList.Add(mailer);
                }
            }

            return mailerList;
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
