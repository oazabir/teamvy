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
            //return GenerateMailerList();
            Dictionary<int, string> lstOfEmailSchedules = unitofWork.EmailSchedulerRepository.GetSchedulerList();

            foreach (KeyValuePair<int,string> item in lstOfEmailSchedules)
            {
                if (item.Key == 1)  //Do for "Remainder email for provide estimation"
                {
                    //Do for "Remainder email for provide estimation"

                   lstTestMailer = EstimationMail();
                }
            }

           
            lstTestMailer.Add(new Mailer { UseMailID = "mahedee.hasan@gmail.com", HtmlMailBody = "This is a test mgs", MailSubject = "Hello msg" });

            return lstTestMailer;
            //return new List<Mailer>();
        }

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
            
            EmailScheduler objEmailScheduler = new EmailScheduler();

            //objEmailScheduler = unitofWork.EmailSchedulerRepository.All.Where(p=>p.SchedulerID == 1).ToList().FirstOrDefault;
            //if(unitofWork.EmailSentStatusRepository.EmailSentStatusBySchedulerId(1).Where(p=>p.ScheduleDateTime == 

            return new List<Mailer>();
        }



        



    }
}
