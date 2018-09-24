namespace Luminator.PresentationPlayLogging.DataStore.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;

    public abstract class EFControllerBase<TConext, T> : IEFController<TConext, T>
        where TConext : DbContext where T : class, IItem
    {
        protected EFControllerBase(TConext context)
        {
            this.Context = context;
        }

        protected EFControllerBase(string connectionString)
        {
            this.Context = new DbContext(connectionString) as TConext;
        }

        public TConext Context { get; set; }

        public virtual IQueryable<T> AsQueryable(Expression<Func<T, bool>> predicate)
        {
            return this.Context.Set<T>().AsQueryable();
        }

        public virtual T Create(T entity)
        {
            var item = this.Context.Set<T>().Add(entity);
            this.Context.SaveChanges();
            return item;
        }

        public virtual void Delete(Guid id)
        {
            var item = this.GetById(id);
            if (item != null)
            {
                this.Context.Set<T>().Remove(item);
                this.Context.SaveChanges();
            }
        }

        public virtual T GetById(Guid id)
        {
            return this.Context.Set<T>().Find(id);
        }

        public virtual T GetByName(string name)
        {
            throw new NotImplementedException();
        }

        public virtual T GetFirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return this.Context.Set<T>().FirstOrDefault(predicate);
        }

        public virtual ICollection<T> GetList()
        {
            return this.Context.Set<T>().AsQueryable().ToList();
        }

        public virtual T GetSingleOrDefault(Expression<Func<T, bool>> predicate)
        {
            return this.Context.Set<T>().SingleOrDefault(predicate);
        }

        public virtual void Update(T entity)
        {
            this.Context.Set<T>().Add(entity);
            this.Context.Entry(entity).State = EntityState.Modified;
            this.Context.SaveChanges();
        }
    }
}