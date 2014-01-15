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
    public class LabelRepository : ILabelRepository
    {
        PMToolContext context = new PMToolContext();


          public  LabelRepository()
            : this(new PMToolContext())
        {
            
        }
          public LabelRepository(PMToolContext context)
        {
            
            this.context = context;
        }

        public IQueryable<Label> All
        {
            get { return context.Labels; }
        }

        public IQueryable<Label> AllIncluding(params Expression<Func<Label, object>>[] includeProperties)
        {
            IQueryable<Label> query = context.Labels;
            foreach (var includeProperty in includeProperties) {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public Label Find(long id)
        {
            return context.Labels.Find(id);
        }

        public void InsertOrUpdate(Label label)
        {
            if (label.LabelID == default(long)) {
                // New entity

                context.Labels.Add(label); 
            } else {
                // Existing entity
                context.Entry(label).State = System.Data.Entity.EntityState.Modified;
            }
        }

        public void Delete(long id)
        {
            var label = context.Labels.Find(id);
            context.Labels.Remove(label);
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

    public interface ILabelRepository : IDisposable
    {
        IQueryable<Label> All { get; }
        IQueryable<Label> AllIncluding(params Expression<Func<Label, object>>[] includeProperties);
        Label Find(long id);
        void InsertOrUpdate(Label label);
        void Delete(long id);
        void Save();
    }
}