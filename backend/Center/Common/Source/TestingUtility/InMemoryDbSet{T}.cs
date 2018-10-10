// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InMemoryDbSet{T}.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InMemoryDbSet type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.TestingUtility
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// Defines an in-memory implementation for the <see cref="IDbSet{TEntity}"/> interface that can
    /// be used mainly for testing purposes.
    /// </summary>
    /// <typeparam name="T">The type of the stored entity.</typeparam>
    public class InMemoryDbSet<T> : IDbSet<T>
        where T : class
    {
        private readonly HashSet<T> data;

        /// <summary>
        /// Stores the current query.
        /// </summary>
        private readonly IQueryable query;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryDbSet&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="entities">The initial entities.</param>
        public InMemoryDbSet(IEnumerable<T> entities)
        {
            this.data = new HashSet<T>(entities);
            this.query = this.data.AsQueryable();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryDbSet&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="entities">The entities.</param>
        public InMemoryDbSet(params T[] entities)
            : this(entities as IEnumerable<T>)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryDbSet&lt;T&gt;"/> class.
        /// </summary>
        public InMemoryDbSet()
        {
            this.data = new HashSet<T>();
            this.query = this.data.AsQueryable();
        }

        /// <summary>
        /// Gets the observable collection of local date.
        /// </summary>
        public System.Collections.ObjectModel.ObservableCollection<T> Local
        {
            get { return new System.Collections.ObjectModel.ObservableCollection<T>(this.data); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether Add operation shouldn't affect the inner data set.
        /// </summary>
        /// <value>
        ///   <c>true</c> if Add operations should be ignored; otherwise, <c>false</c>.
        /// </value>
        public bool IgnoreAdds { get; set; }

        /// <summary>
        /// Gets the type of the element(s) that are returned when the expression tree associated
        /// with this instance of <see cref="T:System.Linq.IQueryable"/> is executed.
        /// </summary>
        /// <returns>A <see cref="T:System.Type"/> that represents the type of the element(s) that
        /// are returned when the expression tree associated with this object is executed.</returns>
        public Type ElementType
        {
            get { return this.query.ElementType; }
        }

        /// <summary>
        /// Gets the expression tree that is associated with the instance of <see cref="T:System.Linq.IQueryable"/>.
        /// </summary>
        /// <returns>The <see cref="T:System.Linq.Expressions.Expression"/> that is
        /// associated with this instance of <see cref="T:System.Linq.IQueryable"/>.</returns>
        public Expression Expression
        {
            get { return this.query.Expression; }
        }

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        /// <returns>The <see cref="T:System.Linq.IQueryProvider"/> that is associated with this data source.</returns>
        public IQueryProvider Provider
        {
            get { return this.query.Provider; }
        }

        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The entity added.</returns>
        public T Add(T entity)
        {
            if (!this.IgnoreAdds)
            {
                this.data.Add(entity);
            }

            return entity;
        }

        /// <summary>
        /// Attaches the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The entity attached.</returns>
        public T Attach(T entity)
        {
            this.data.Add(entity);
            return entity;
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <typeparam name="TDerivedEntity">The type of the derived entity.</typeparam>
        /// <returns>The derived entity created.</returns>
        public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates an object of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>The object created.</returns>
        public T Create()
        {
            return Activator.CreateInstance<T>();
        }

        /// <summary>
        /// Finds the specified key values.
        /// </summary>
        /// <param name="keyValues">The key values.</param>
        /// <returns>The found object.</returns>
        public virtual T Find(params object[] keyValues)
        {
            throw new NotImplementedException("Derive from FakeDbSet and override Find");
        }

        /// <summary>
        /// Removes the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The removed entity.</returns>
        public T Remove(T entity)
        {
            this.data.Remove(entity);
            return entity;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can
        /// be used to iterate through the collection.
        /// </returns>
        public IEnumerator<T> GetEnumerator()
        {
            return this.data.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.data.GetEnumerator();
        }
    }
}
