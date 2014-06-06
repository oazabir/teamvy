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

                        if (objOfEmailSent.Count > 0)
                        {
                            foreach (var ObjofList in objOfEmailSent)
                            {

                                bool emailStatus = unitofWork.EmailSentStatusRepository.EmailSentStatus(ObjofList.EmailSchedulerID, ObjofList.ScheduleTypeID, scheduleDateTime);
                                if (emailStatus == true && currentdateTime >= scheduleDateTime)
                                {
                                    continue;
                                }
                                else if(currentdateTime<=scheduleDateTime)
                                {
                                    continue;
                                }

                                else
                                {
                                    objOflstMailer = DailyTaskStatus(emailSchedule);
                                    //objOflstMailer.AddRange(DailyTaskStatusForCreator());

                                    if (objOflstMailer.Count >= 0)
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
                            if (currentdateTime >= scheduleDateTime) 
                            {
                                objOflstMailer = DailyTaskStatus(emailSchedule);
                                //objOflstMailer.AddRange(DailyTaskStatusForCreator());

                                if (objOflstMailer.Count >= 0)
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

                else if (emailSchedule.SchedulerTitleID == 4) // This is for Notification Email
                {
                    DateTime scheduleDateTime = DateTime.ParseExact(emailSchedule.ScheduledTime, "h:mm tt", CultureInfo.InvariantCulture);
                    //List<Notification> ListOfNotification = unitofWork.NotificationRepository.GetNotificationDetails(emailSchedule);              
  
                    if (objOfEmailSent.Count >0)
                    {

                        foreach (var objOfEmailSentStutusDet in objOfEmailSent) 
                        {

                            bool emailStatus = unitofWork.EmailSentStatusRepository.EmailSentStatus(objOfEmailSentStutusDet.EmailSchedulerID, objOfEmailSentStutusDet.ScheduleTypeID, scheduleDateTime);

                            if (emailStatus == true && currentdateTime>scheduleDateTime)
                            {
                                continue;
                            }
                            else if(currentdateTime<=scheduleDateTime)
                            {
                                continue;
                            }
                           
                            else
                            {
                                objOflstMailer = NotificationEmail(emailSchedule.ProjectID, scheduleDateTime); // Mail Function Call here

                                if (objOflstMailer.Count >= 0)
                                {
                                    EmailSentStatus objEmailSentStatus = new EmailSentStatus();
                                    objEmailSentStatus.EmailSchedulerID = 4;
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
                        if (currentdateTime >= scheduleDateTime) 
                        {
                            objOflstMailer = NotificationEmail(emailSchedule.ProjectID, scheduleDateTime); // Mail Function Call here

                            if (objOflstMailer.Count >= 0)
                            {
                                EmailSentStatus objEmailSentStatus = new EmailSentStatus();
                                objEmailSentStatus.EmailSchedulerID = 4;
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
            //List<UserProfile> userList = unitofWork.UserRepository.All();          

            //string emailbody = "style=\".emailTbl {border: 1px solid #000000;border-radius: 5px;margin: 0;padding: 0;width: 574px;-moz-border-radius-bottomleft:5px;-webkit-border-bottom-left-radius:5px;border-bottom-left-radius:5px;-moz-border-radius-bottomright:5px;-webkit-border-bottom-right-radius:5px;border-bottom-right-radius:5px;-moz-border-radius-topright:5px;-webkit-border-top-right-radius:5px;border-top-right-radius:5px;-moz-border-radius-topleft:5px;-webkit-border-top-left-radius:5px;border-top-left-radius:5px;}.emailTbl table{border-collapse: collapse;border-spacing: 0;width:100%;margin:0px;padding:0px;}.emailTbl tr:last-child td:last-child {-moz-border-radius-bottomright:5px;-webkit-border-bottom-right-radius:5px;border-bottom-right-radius:5px;}.emailTbl table tr:first-child td:first-child {-moz-border-radius-topleft:5px;-webkit-border-top-left-radius:5px;border-top-left-radius:5px;}.emailTbl table tr:first-child td:last-child {-moz-border-radius-topright:5px;-webkit-border-top-right-radius:5px;border-top-right-radius:5px;}.emailTbl tr:last-child td:first-child{-moz-border-radius-bottomleft:5px;-webkit-border-bottom-left-radius:5px;border-bottom-left-radius:5px;}.emailTbl tr:hover td{}.emailTbl tr:nth-child(odd){ background-color:#e8edff; }.emailTbl tr:nth-child(even){background-color:#ffffff; }.emailTbl td{vertical-align:middle;border:1px solid #000000;border-width:0px 1px 1px 0px;text-align:left;padding:9px;font-size:12px;font-family:Arial;font-weight:normal;color:#000000;}.emailTbl tr:last-child td{border-width:0px 1px 0px 0px;}.emailTbl tr td:last-child{border-width:0px 0px 1px 0px;}.emailTbl tr:last-child td:last-child{border-width:0px 0px 0px 0px;}.emailTbl tr:first-child td{background:-o-linear-gradient(bottom, #b9c9fe 5%, #b9c9fe 100%);	background:-webkit-gradient( linear, left top, left bottom, color-stop(0.05, #b9c9fe), color-stop(1, #b9c9fe) );background:-moz-linear-gradient( center top, #b9c9fe 5%, #b9c9fe 100% );filter:progid:DXImageTransform.Microsoft.gradient(startColorstr=#b9c9fe,endColorstr=#b9c9fe);	background: -o-linear-gradient(top,#b9c9fe,b9c9fe);background-color:#b9c9fe;border:0px solid #000000;text-align:center;border-width:0px 0px 1px 1px;font-size:14px;font-family:Arial;font-weight:bold;color:#000000;}.emailTbl tr:first-child:hover td{background:-o-linear-gradient(bottom, #b9c9fe 5%, #b9c9fe 100%);	background:-webkit-gradient( linear, left top, left bottom, color-stop(0.05, #b9c9fe), color-stop(1, #b9c9fe) );background:-moz-linear-gradient( center top, #b9c9fe 5%, #b9c9fe 100% );filter:progid:DXImageTransform.Microsoft.gradient(startColorstr=#b9c9fe, endColorstr=#b9c9fe);	background: -o-linear-gradient(top,#b9c9fe,b9c9fe);background-color:#b9c9fe;}.emailTbl tr:first-child td:first-child{border-width:0px 0px 1px 0px;}.emailTbl tr:first-child td:last-child{border-width:0px 0px 1px 1px;}\"";
            //string styleTable     = "style=\"border-collapse: collapse;border-spacing: 0;width: 100%;margin: 0;padding: 0\"";
            //string styleTableRow  = "style=\"background-color: #fff\"";
            //string styleTableData = "style=\"vertical-align: middle;border: 1px solid #000;border-width: 0 1px 1px 0;text-align: left;padding: 9px;font-size: 12px;font-family: Arial;font-weight: normal;color: #000\"";

            List<UserProfile> userList = new List<UserProfile>();

            if (objEmailScheduler.RecipientUserType.Value == 1)             //Task's user
            {
               List<UserProfile> userListByProject = GetAllUserByProjectId(objEmailScheduler.ProjectID);
               userList = userListByProject;
            }
            else if (objEmailScheduler.RecipientUserType.Value == 2)       // Task's Followers
            {
                List<UserProfile> userListByProject = GetAllUserByProjectId(objEmailScheduler.ProjectID);
                userList = userListByProject;
            }
            else if (objEmailScheduler.RecipientUserType.Value == 3)        //Task's Users & Followers
            {
                List<UserProfile> userListByProject = GetAllUserByProjectId(objEmailScheduler.ProjectID);
                userList = userListByProject;
            }
            else if (objEmailScheduler.RecipientUserType.Value == 4)        //Task's Creator & Followers
            {
                List<UserProfile> userListByProject = GetAllUserByProjectId(objEmailScheduler.ProjectID);
                userList = userListByProject;
            }
            else if (objEmailScheduler.RecipientUserType.Value == 5)        //Project's Users
            {
                List<UserProfile> userListByProject = GetAllUserByProjectId(objEmailScheduler.ProjectID);
                userList = userListByProject;
            }

            else if (objEmailScheduler.RecipientUserType.Value == 6)        //Task's Creator & Users
            {
                List<UserProfile> userListByProject = GetAllUserByProjectId(objEmailScheduler.ProjectID);
                userList = userListByProject;
            }

            else if (objEmailScheduler.RecipientUserType.Value == 7)        //Task's Creator & Users
            {
                List<UserProfile> userListByProject = GetAllUserByProjectId(objEmailScheduler.ProjectID);
                userList = userListByProject;
            }


            string styleTableHeader = "style= \"background-color:#0094ff; border:1px solid;\"";
            string styleGroupbyRow = "style= \"background-color:#57C0E1;\"";
            string styleTaskRow = "style= \"background-color:#A0D0FF;\"";

            foreach(UserProfile objOfuser in userList)
            {
                string messageBody = string.Empty;
                List<Task> userTaskList = new List<Task>();
                           
                if (objEmailScheduler.RecipientUserType.Value == 1)  //Task's user
                {
                    userTaskList= taskList.Where(a => a.Users.Any(b => b.UserId == objOfuser.UserId) && a.ProjectID == objEmailScheduler.ProjectID).ToList();
                }

                else if (objEmailScheduler.RecipientUserType.Value == 2)// Task's Followers
                {
                    userTaskList = taskList.Where(a => a.Followers.Any(b => b.UserId == objOfuser.UserId) && a.ProjectID == objEmailScheduler.ProjectID).ToList();
                }

                else if (objEmailScheduler.RecipientUserType.Value == 3) //Task's Users & Followers
                {
                    userTaskList = taskList.Where(a => a.Followers.Any(b => b.UserId == objOfuser.UserId && a.ProjectID == objEmailScheduler.ProjectID) || (a.Users.Any(c => c.UserId == objOfuser.UserId && a.ProjectID == objEmailScheduler.ProjectID))).ToList();
                }
                else if (objEmailScheduler.RecipientUserType.Value == 4) //Task's Creator & Followers
                {
                    userTaskList = taskList.Where((a => a.CreatedBy == objOfuser.UserId && a.ProjectID == objEmailScheduler.ProjectID || a.Followers.Any(b => b.UserId == objOfuser.UserId && a.ProjectID == objEmailScheduler.ProjectID))).ToList();
                }

                else if (objEmailScheduler.RecipientUserType.Value == 5) //Project's Users
                {
                    List<Task> tasklist = unitofWork.TaskRepository.AllTaskByIndividalProject(objEmailScheduler.ProjectID);               
                    userTaskList = tasklist.Where(a => a.Users.Any(b => b.UserId == objOfuser.UserId)).ToList();
                }
                else if (objEmailScheduler.RecipientUserType.Value == 6) //Task's Creator & Users
                {                 
                    userTaskList = taskList.Where((a => a.CreatedByUser == objOfuser && a.ProjectID == objEmailScheduler.ProjectID || a.Users.Any(b => b.UserId == objOfuser.UserId && a.ProjectID==objEmailScheduler.ProjectID))).ToList();
                }

                else if (objEmailScheduler.RecipientUserType.Value == 7) //Task's Creator 
                {                   
                    userTaskList = taskList.Where(a => a.CreatedBy == objOfuser.UserId && a.ProjectID==objEmailScheduler.ProjectID).ToList();
                }

               
                //taskList.Where(a => a.Users.Any(b => b.UserId == objOfuser.UserId) && a.ProjectID == objEmailScheduler.ProjectID).ToList();

                if (userTaskList.Count > 0)
                {
                    messageBody = "<b>Dear &nbsp;" + objOfuser.FirstName + "</b>,<br>" + "<b>Your Daily Tasks Status are given below</b><br>";
                   
                    messageBody += "<table><tr " + styleTableHeader + "><th>Task ID</th> <th>Task Title</th> <th>Task Status</th> <th>Project</th></tr>";

                    foreach (var task in userTaskList)
                    {
                        messageBody += "<tr> ";
                        messageBody += "<td " + styleTaskRow + "> " + task.TaskUID + "</td>";
                        messageBody += "<td " + styleGroupbyRow + "> " + task.Title + "</td>";
                        messageBody += "<td " + styleGroupbyRow + "> " + ((task.ProjectStatus == null) ? " " : task.ProjectStatus.Name) + "</td>";
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


        /// <summary>
        /// This method Return the all User of Specific Project
        /// Created By - Foysal 
        /// Date : 05/06/2014
        /// </summary>
        /// <param name="ProjectID"></param>
        /// <returns></returns>
        private List<UserProfile> GetAllUserByProjectId(long ProjectID)
        {
            //List<UserProfile> allUsers = new List<UserProfile>();
            //List<UserProfile> userList = unitofWork.ProjectRepository.Find(ProjectID).Users;
            //foreach (UserProfile user in userList)
            //{
            //    UserProfile users = new UserProfile { UserId = user.UserId ,UserName = user.UserName ,FirstName=user.FirstName,LastName=user.LastName,Email=user.Email};
            //    allUsers.Add(users);
            //}
            //return allUsers;

            List<UserProfile> userList = unitofWork.ProjectRepository.Find(ProjectID).Users;
            return userList;
        }


        /* 
         Send Notification Email to the User if any update occur in task update
         */

        public List<Mailer> NotificationEmail(long projectId, DateTime scheduleDt) 
        {
            List<EmailSentStatus> lstEmailSent = unitofWork.EmailSentStatusRepository.All.Where(p => p.ScheduleTypeID == 1 && p.EmailSchedulerID == 4).ToList();
            var maxSentDate = (from m in lstEmailSent
                               select m.ScheduleDateTime).Max();

            if (lstEmailSent.Count != 0)
            {
                //List<Notification> ListOfNotification = unitofWork.NotificationRepository.AllIncluding().Where(p => p.ActionDate > maxSentDate && p.ActionDate <= scheduleDt && p.ProjectID == projectId).ToList();
                List<Mailer> mailerList = new List<Mailer>();
                List<UserProfile> userList = unitofWork.UserRepository.All();

                string styleTableHeader = "style= \"background-color:#0094ff; border:1px solid;\"";
                string styleGroupbyRow = "style= \"background-color:#57C0E1;\"";

                foreach (UserProfile objOfuser in userList)
                {
                    List<Notification> ListOfNotification = unitofWork.NotificationRepository.AllIncluding().Where(p => p.ActionDate > maxSentDate && p.ActionDate <= scheduleDt && p.ProjectID == projectId).ToList();
                    List<Notification> userTasksNotificationList = ListOfNotification.Where(p => p.Task.Users.Any(u => u.UserId == objOfuser.UserId) || p.Task.CreatedBy == objOfuser.UserId).ToList();

                    List<Task> userTask = unitofWork.TaskRepository.All.Where(p => (p.Users.Any(u => u.UserId == objOfuser.UserId) || p.CreatedBy == objOfuser.UserId) && p.ProjectStatus.Name.ToLower() != "closed").ToList();
                        //.Where(a => a.UserID == objOfuser.UserId).ToList();
                    List<Task> userTaskwithNoChange = userTask.Where(p => !userTasksNotificationList.Any(p2 => p2.TaskID == p.TaskID)).ToList();

                    string messageBody = string.Empty;

                    if(userTaskwithNoChange.Count>0)
                    {

                        messageBody = "<b>Dear &nbsp;" + objOfuser.FirstName + "</b>,<br>" + "<b>Your Task Changes History is Given Below</b><br>";
                        messageBody += "<table><tr " + styleTableHeader + "><th>Task ID</th><th>Task Title</th><th>Change History</th><th>Modifying Date</th></tr>";

                        foreach (var Notificationlst in userTaskwithNoChange)
                        {

                            messageBody += "<tr>";
                            messageBody += "<td " + styleGroupbyRow + "> " + ((Notificationlst.TaskUID == null) ? " " : Notificationlst.TaskUID) + "</td>";
                            messageBody += "<td " + styleGroupbyRow + "> " + ((Notificationlst.Title == null) ? " " : Notificationlst.Title) + "</td>";
                            messageBody += "<td " + styleGroupbyRow + "> No Changes" + "</td>";
                            messageBody += "<td " + styleGroupbyRow + "> " + Notificationlst.ActionDate + "</td>";
                            messageBody += "</tr>";
                        }
                        messageBody += "</table></div><br /><div style='float:left;'><p>We sent you this email because you signed up in PMTool and tasks are assigned to you. <br /> Please don't reply To this mail.</p><p>"
                        + "Regards,<br />PMTool</p></div>";

                        Mailer mailer = new Mailer();
                        mailer.UseMailID = objOfuser.UserName;
                        mailer.MailSubject = "Task Changes history Notification";
                        mailer.HtmlMailBody = messageBody;
                        mailerList.Add(mailer);
                    }

                    if (userTasksNotificationList.Count > 0)
                    {

                        messageBody = "<b>Dear &nbsp;" + objOfuser.FirstName + "</b>,<br>" + "<b>Your Task Changes History is Given Below</b><br>";
                        messageBody += "<table><tr " + styleTableHeader + "><th>Task ID</th><th>Task Title</th><th>Change History</th><th>Modifying Date</th></tr>";

                        foreach (var Notificationlst in userTasksNotificationList)
                        {

                            messageBody += "<tr>";
                            messageBody += "<td " + styleGroupbyRow + "> " + ((Notificationlst.Task == null) ? " " : Notificationlst.Task.TaskUID) + "</td>";
                            messageBody += "<td " + styleGroupbyRow + "> " + ((Notificationlst.Task == null) ? " " : Notificationlst.Task.Title) + "</td>";
                            messageBody += "<td " + styleGroupbyRow + "> " + Notificationlst.Description + "</td>";
                            messageBody += "<td " + styleGroupbyRow + "> " + Notificationlst.ActionDate + "</td>";
                            messageBody += "</tr>";
                        }
                        messageBody += "</table></div><br /><div style='float:left;'><p>We sent you this email because you signed up in PMTool and tasks are assigned to you. <br /> Please don't reply To this mail.</p><p>"
                        + "Regards,<br />PMTool</p></div>";

                        Mailer mailer = new Mailer();
                        mailer.UseMailID = objOfuser.UserName;
                        mailer.MailSubject = "Task Changes history Notification";
                        mailer.HtmlMailBody = messageBody;
                        mailerList.Add(mailer);
                    }

                   
                }

                return mailerList;

            }

            else 
            {
                List<Notification> ListOfNotification = unitofWork.NotificationRepository.AllIncluding().Where(p =>p.ActionDate >= scheduleDt && p.ProjectID == projectId).ToList();
                List<Mailer> mailerList = new List<Mailer>();
                List<UserProfile> userList = unitofWork.UserRepository.All();

                string styleTableHeader = "style= \"background-color:#0094ff; border:1px solid;\"";
                string styleGroupbyRow = "style= \"background-color:#57C0E1;\"";

                foreach (UserProfile objOfuser in userList)
                {
                    string messageBody = string.Empty;
                    List<Notification> userTasksNotificationList = ListOfNotification.Where(a => a.UserID == objOfuser.UserId).ToList();


                    if (userTasksNotificationList.Count > 0)
                    {

                        messageBody = "<b>Dear &nbsp;" + objOfuser.FirstName + "</b>,<br>" + "<b>Your Task Changes History is Given Below</b><br>";
                        messageBody += "<table><tr " + styleTableHeader + "><th>Task ID</th><th>Task Title</th><th>Change History</th><th>Modifying Date</th></tr>";

                        foreach (var Notificationlst in userTasksNotificationList)
                        {

                            messageBody += "<tr>";
                            messageBody += "<td " + styleGroupbyRow + "> " + ((Notificationlst.Task == null) ? " " : Notificationlst.Task.TaskUID) + "</td>";
                            messageBody += "<td " + styleGroupbyRow + "> " + ((Notificationlst.Task == null) ? " " : Notificationlst.Task.Title) + "</td>";
                            messageBody += "<td " + styleGroupbyRow + "> " + Notificationlst.Description + "</td>";
                            messageBody += "<td " + styleGroupbyRow + "> " + Notificationlst.ActionDate + "</td>";
                            messageBody += "</tr>";
                        }
                        messageBody += "</table></div><br /><div style='float:left;'><p>We sent you this email because you signed up in PMTool and tasks are assigned to you. <br /> Please don't reply To this mail.</p><p>"
                        + "Regards,<br />PMTool</p></div>";

                        Mailer mailer = new Mailer();
                        mailer.UseMailID = objOfuser.UserName;
                        mailer.MailSubject = "Task Changes history Notification";
                        mailer.HtmlMailBody = messageBody;
                        mailerList.Add(mailer);
                    }

                }
                return mailerList;
            }

            //List<Notification> ListOfNotification = unitofWork.NotificationRepository.AllIncluding().Where(p => p.ActionDate > maxSentDate && p.ActionDate <= scheduleDt && p.ProjectID == projectId).ToList();           
            //List<Mailer> mailerList = new List<Mailer>();          
            //List<UserProfile> userList = unitofWork.UserRepository.All();

            //string styleTableHeader = "style= \"background-color:#0094ff; border:1px solid;\"";
            //string styleGroupbyRow = "style= \"background-color:#57C0E1;\"";

            //foreach (UserProfile objOfuser in userList)
            //{
            //    string messageBody = string.Empty;
            //    List<Notification> userTasksNotificationList = ListOfNotification.Where(a => a.UserID == objOfuser.UserId).ToList();
                
                
            //    if(userTasksNotificationList.Count>0)
            //    {

            //        messageBody = "<b>Dear &nbsp;" + objOfuser.FirstName + "</b>,<br>" + "<b>Your Task Changes History is Given Below</b><br>";
            //        messageBody += "<table><tr " + styleTableHeader + "><th>Task ID</th><th>Task Title</th><th>Description</th><th>Modifying Date</th></tr>";

            //        foreach(var Notificationlst in userTasksNotificationList)
            //        {
                       
            //            messageBody += "<tr>";
            //            messageBody += "<td " + styleGroupbyRow + "> " + ((Notificationlst.Task == null) ? " " : Notificationlst.Task.TaskUID) + "</td>";
            //            messageBody += "<td " + styleGroupbyRow + "> " + ((Notificationlst.Task == null) ? " " : Notificationlst.Task.Title) + "</td>";
            //            messageBody += "<td " + styleGroupbyRow + "> " + Notificationlst.Description + "</td>";
            //            messageBody += "<td " + styleGroupbyRow + "> " + Notificationlst.ActionDate + "</td>";
            //            messageBody += "</tr>";
            //        }
            //        messageBody += "</table></div><br /><div style='float:left;'><p>We sent you this email because you signed up in PMTool and tasks are assigned to you. <br /> Please don't reply To this mail.</p><p>"
            //        + "Regards,<br />PMTool</p></div>";

            //        Mailer mailer = new Mailer();
            //        mailer.UseMailID = objOfuser.UserName;
            //        mailer.MailSubject = "Task Changes history Notification";
            //        mailer.HtmlMailBody = messageBody;
            //        mailerList.Add(mailer);
            //    }

            //}

            //return mailerList;
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
