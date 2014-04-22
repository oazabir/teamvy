using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using PMTool.Repository;
using PMTool.Models;

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


        public Dictionary<int, string> GetSchedulerList()
        {
            var lstOfEmailSchedules = new Dictionary<int, string> {{ 1, "Reminder mail to provide estimation" }, 
                                                                { 2, "Optional scheduler" }, };

            return lstOfEmailSchedules;
        }

        public void InsertOrUpdate(EmailScheduler emailscheduler)
        {
            if (emailscheduler.SchedulerID == default(long)) {
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