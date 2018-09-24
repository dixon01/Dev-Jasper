namespace Luminator.PresentationPlayLogging.DataStore.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>The EFController interface.</summary>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="T"></typeparam>
    public interface IEFController<TContext, T>
        where TContext : DbContext where T : IItem
    {
        TContext Context { get; set; }

        T Create(T entity);

        void Delete(Guid id);

        ICollection<T> GetList();

        T GetById(Guid id);

        T GetByName(string name);

        void Update(T entity);

        T GetSingleOrDefault(Expression<Func<T, bool>> predicate);

        T GetFirstOrDefault(Expression<Func<T, bool>> predicate);

        IQueryable<T> AsQueryable(Expression<Func<T, bool>> predicate);
    }
}