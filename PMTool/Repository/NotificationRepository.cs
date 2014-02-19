using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using PMTool.Models;
using PMTool.Repository;

namespace PMTool.Repository
{ 
    public class NotificationRepository : INotificationRepository
    {
        PMToolContext context = new PMToolContext();
        
          public  NotificationRepository()
            : this(new PMToolContext())
        {
            
        }
          public NotificationRepository(PMToolContext context)
        {
            
            this.context = context;
        }

        public IQueryable<Notification> All
        {
            get { return context.Notifications; }
        }

        public IQueryable<Notification> AllIncluding(params Expression<Func<Notification, object>>[] includeProperties)
        {
            IQueryable<Notification> query = context.Notifications;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Notification Find(long id)
        {
            return context.Notifications.Find(id);
        }

        public void InsertOrUpdate(Notification notification)
        {
            if (notification.NotificationID == default(long)) {
                // New entity
                context.Notifications.Add(notification);
            } else {
                // Existing entity
                context.Entry(notification).State = System.Data.Entity.EntityState.Modified;
            }
        }

        public void Delete(long id)
        {
            var notification = context.Notifications.Find(id);
            context.Notifications.Remove(notification);
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Dispose() 
        {
            context.Dispose();
        }

        public List<Notification> UserUnreadNotification(User user)
        {
          return  context.Notifications.Where(n=>n.UserID==user.UserId && n.IsNoticed==false).ToList();
        }

        internal List<Notification> FindNotification(long ProjectID)
        {
            List<Notification> notification = context.Notifications.Where(n => n.ProjectID == ProjectID).ToList();
            return notification;
        }
    }

    public interface INotificationRepository : IDisposable
    {
        IQueryable<Notification> All { get; }
        IQueryable<Notification> AllIncluding(params Expression<Func<Notification, object>>[] includeProperties);
        Notification Find(long id);
        void InsertOrUpdate(Notification notification);
        void Delete(long id);
        void Save();
        List<Notification> UserUnreadNotification(User user);
    }
}