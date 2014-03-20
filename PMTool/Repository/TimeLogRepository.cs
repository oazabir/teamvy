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
    /// <summary>
    /// Repository for TimeLog
    /// Created by Mahedee @18-03-14
    /// </summary>
    public class TimeLogRepository : ITimeLogRepository
    {
        PMToolContext context = new PMToolContext();

        public TimeLogRepository()
            : this(new PMToolContext())
        {

        }
        public TimeLogRepository(PMToolContext context)
        {

            this.context = context;
        }


        public IQueryable<TimeLog> All
        {
            get { return context.TimeLogs; }
        }

        public IQueryable<TimeLog> AllIncluding(params Expression<Func<TimeLog, object>>[] includeProperties)
        {
            IQueryable<TimeLog> query = context.TimeLogs;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public List<TimeLog> GetTimeLogBySprint(long sprintID)
        {
            List<TimeLog> timeLogList = context.TimeLogs.Where(t => t.SprintID == sprintID).ToList();
            return timeLogList;
        }

        public TimeLog Find(long id)
        {
            return context.TimeLogs.Find(id);
        }

        public void InsertOrUpdate(TimeLog timelog)
        {
            if (timelog.LogID == default(long)) {
                // New entity
                context.TimeLogs.Add(timelog);
            } else {
                // Existing entity
                //context.Entry(timelog).State = System.Data.EntityState.Modified;
                context.Entry(timelog).State = System.Data.Entity.EntityState.Modified;
            }
        }

        public void Delete(long id)
        {
            var timelog = context.TimeLogs.Find(id);
            context.TimeLogs.Remove(timelog);
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

    public interface ITimeLogRepository : IDisposable
    {
        IQueryable<TimeLog> All { get; }
        IQueryable<TimeLog> AllIncluding(params Expression<Func<TimeLog, object>>[] includeProperties);
        TimeLog Find(long id);
        void InsertOrUpdate(TimeLog timelog);
        void Delete(long id);
        void Save();
    }
}