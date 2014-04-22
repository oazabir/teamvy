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
    public class EmailSentStatusRepository : IEmailSentStatusRepository
    {
       
        //PMToolContext context = new PMToolContext();

        PMToolContext context = new PMToolContext();

        public EmailSentStatusRepository()
            : this(new PMToolContext())
        {

        }
        public EmailSentStatusRepository(PMToolContext context)
        {

            this.context = context;
        }


        public IQueryable<EmailSentStatus> All
        {
            get { return context.EmailSentStatus; }
        }

        public List<EmailSentStatus> EmailSentStatusBySchedulerId(long schedulerId)
        {
            return context.EmailSentStatus.Where(p => p.EmailSchedulerID == schedulerId).ToList();
        }

        public IQueryable<EmailSentStatus> AllIncluding(params Expression<Func<EmailSentStatus, object>>[] includeProperties)
        {
            IQueryable<EmailSentStatus> query = context.EmailSentStatus;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public EmailSentStatus Find(long id)
        {
            return context.EmailSentStatus.Find(id);
        }

        public void InsertOrUpdate(EmailSentStatus emailsentstatus)
        {
            if (emailsentstatus.ID == default(long)) {
                // New entity
                context.EmailSentStatus.Add(emailsentstatus);
            } else {
                // Existing entity
                context.Entry(emailsentstatus).State = System.Data.Entity.EntityState.Modified;
            }
        }

        public void Delete(long id)
        {
            var emailsentstatus = context.EmailSentStatus.Find(id);
            context.EmailSentStatus.Remove(emailsentstatus);
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

    public interface IEmailSentStatusRepository : IDisposable
    {
        IQueryable<EmailSentStatus> All { get; }
        IQueryable<EmailSentStatus> AllIncluding(params Expression<Func<EmailSentStatus, object>>[] includeProperties);
        EmailSentStatus Find(long id);
        void InsertOrUpdate(EmailSentStatus emailsentstatus);
        void Delete(long id);
        void Save();
    }
}