using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using PMTool.Repository;
using PMTool.Models;
using System.Web.Mvc;

namespace PMTool.Repository
{ 
    public class EmailSchedulerRepository : IEmailSchedulerRepository
    {
        //PMToolContext context = new PMToolContext();

        PMToolContext context = new PMToolContext();

        public EmailSchedulerRepository()
            : this(new PMToolContext())
        {

        }
        public EmailSchedulerRepository(PMToolContext context)
        {

            this.context = context;
        }



        public IQueryable<EmailScheduler> All
        {
            get { return context.EmailSchedulers; }
        }

        public IQueryable<EmailScheduler> AllIncluding(params Expression<Func<EmailScheduler, object>>[] includeProperties)
        {
            IQueryable<EmailScheduler> query = context.EmailSchedulers;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public EmailScheduler Find(long id)
        {
            return context.EmailSchedulers.Find(id);
        }



        public List<EmailScheduler> GetEmailSchedulerAll()
        {
            List<EmailScheduler> emailschedulerlst = this.AllIncluding().ToList();

            foreach (var item in emailschedulerlst)
            {
                item.SchedulerTitles = from title in GetSchedulerList()
                                       where title.Key == item.SchedulerTitleID.ToString()
                                                 select new SelectListItem
                                                 {
                                                     Text = title.Value,
                                                     Value = title.Key
                                                 };

                //IEnumerable<ScheduleType> scheduleTypes = Enum.GetValues(typeof(ScheduleType)).Cast<ScheduleType>();
                item.ScheduleType = from stype in GetSchedulerTypeAll() where stype.Key == item.ScheduleTypeID.ToString()
                                              select new SelectListItem
                                              {
                                                  Text = stype.Value,
                                                  Value = stype.Key
                                              };

                item.EmailRecipientUsers = from rutype in GetRecipientUserTypeAll()
                                           where rutype.Key == item.RecipientUserType.ToString()
                                    select new SelectListItem
                                    {
                                        Text = rutype.Value,
                                        Value = rutype.Key
                                    };

                item.Days = from days in GetDaysOfWeek()
                            where days.Key == item.ScheduledDay.ToString()
                                           select new SelectListItem
                                           {
                                               Text = days.Value,
                                               Value = days.Key
                                           };



            }

            return emailschedulerlst;
        }

        public Dictionary<string, string> GetDaysOfWeek()
        {
            return new Dictionary<string, string> 
            {{"0", "---Select Day---"},
             {"1", "Saturday"},
             {"2", "Sunday"},
             {"3", "Monday"},
             {"4", "Tuesday"},
             {"5", "Wednesday"},
             {"6", "Thursday"},
             {"7", "Friday"}

            };
          }
        public Dictionary<string, string> GetRecipientUserTypeAll()
        {

            return new Dictionary<string, string> {{ "0", "--- Select recipient users ---" }, 
                                                                { "1", "Task's Users" }, 
                                                                { "2", "Task's Followers" },
                                                                { "3", "Task's Users & Followers" },
                                                                 { "4", "Project's Users" }};
        }

        public Dictionary<string, string> GetSchedulerTypeAll()
        {
            var lstOfScheduleType = new Dictionary<string, string> {
            {"0", "---Select Type---"},
            { "1", "Daily" }, 
            { "2", "Weekly" },
            { "3", "Monthly" }, };

            return lstOfScheduleType;
        }

        public Dictionary<string, string> GetSchedulerList()
        {
            var lstOfEmailSchedules = new Dictionary<string, string> {
            {"0", "---Select Title---"},
            { "1", "Reminder mail to provide estimation" }, 
            { "2", "Daily status mail" },
            { "3", "Daily digest mail" }, };

            return lstOfEmailSchedules;
        }

        public void InsertOrUpdate(EmailScheduler emailscheduler)
        {
            if (emailscheduler.ID == default(long)) {
                // New entity
                context.EmailSchedulers.Add(emailscheduler);
            } else {
                // Existing entity
                context.Entry(emailscheduler).State = System.Data.Entity.EntityState.Modified;
            }


        }

        public void Delete(long id)
        {
            var emailscheduler = context.EmailSchedulers.Find(id);
            context.EmailSchedulers.Remove(emailscheduler);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Dispose() 
        {
            context.Dispose();
        }
    }

    public interface IEmailSchedulerRepository : IDisposable
    {
        IQueryable<EmailScheduler> All { get; }
        IQueryable<EmailScheduler> AllIncluding(params Expression<Func<EmailScheduler, object>>[] includeProperties);
        EmailScheduler Find(long id);
        void InsertOrUpdate(EmailScheduler emailscheduler);
        void Delete(long id);
        void Save();
    }
}