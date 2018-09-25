// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FakeDbSet.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FakeDbSet type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Fake implementation of the <see cref="IDbSet&lt;T&gt;"/>.
    /// <see cref="http://entityframework.codeplex.com/discussions/435590"/>
    /// </summary>
    /// <typeparam name="T">The type of the entities in the set.</typeparam>
    public class FakeDbSet<T> : IDbSet<T>, IDbAsyncEnumerable<T>
        where T : class
    {
        private readonly ObservableCollection<T> data;

        private readonly IQueryable queryable;

        /// <summary>
        /// Initializes a new instance of the <see cref="FakeDbSet{T}"/> class.
        /// </summary>
        public FakeDbSet()
        {
            this.data = new ObservableCollection<T>();
            this.queryable = this.data.AsQueryable();
        }

        Expression IQueryable.Expression
        {
            get
            {
                return this.queryable.Expression;
            }
        }

        IQueryProvider IQueryable.Provider
        {
            get
            {
                return new AsyncQueryProviderWrapper<T>(this.queryable.Provider);
            }
        }

        Type IQueryable.ElementType
        {
            get
            {
                return this.queryable.ElementType;
            }
        }

        /// <summary>
        /// Gets an <see cref="T:System.Collections.ObjectModel.ObservableCollection`1"/> that represents a local view
        /// of all Added, Unchanged, and Modified entities in this set. This local view will stay in sync as entities
        /// are added or removed from the context. Likewise, entities added to or removed from the local view will
        /// automatically be added to or removed from the context.
        /// </summary>
        /// <remarks>
        /// This property can be used for data binding by populating the set with data, for example by using the Load
        /// extension method, and then binding to the local data through this property.  For WPF bind to this property
        /// directly.  For Windows Forms bind to the result of calling ToBindingList on this property
        /// </remarks>
        /// <value>
        /// The local view.
        /// </value>
        public ObservableCollection<T> Local
        {
            get
            {
                return this.data;
            }
        }

        /// <summary>
        /// Finds an entity with the given primary key values.
        ///             If an entity with the given primary key values exists in the context, then it is
        ///             returned immediately without making a request to the store.  Otherwise, a request
        ///             is made to the store for an entity with the given primary key values and this entity,
        ///             if found, is attached to the context and returned.  If no entity is found in the
        ///             context or the store, then null is returned.
        /// </summary>
        /// <remarks>
        /// The ordering of composite key values is as defined in the EDM, which is in turn as defined in
        ///             the designer, by the Code First fluent API, or by the DataMember attribute.
        /// </remarks>
        /// <param name="keyValues">The values of the primary key for the entity to be found. </param>
        /// <returns>
        /// The entity found, or null.
        /// </returns>
        public virtual T Find(params object[] keyValues)
        {
            throw new NotImplementedException("Derive from FakeDbSet<T> and override Find");
        }

        /// <summary>
        /// Finds an item.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="keyValues">The key values.</param>
        /// <returns>The item found, if any.</returns>
        public Task<T> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Adds the given entity to the context underlying the set in the Added state such that it will
        ///             be inserted into the database when SaveChanges is called.
        /// </summary>
        /// <param name="entity">The entity to add. </param>
        /// <returns>
        /// The entity.
        /// </returns>
        /// <remarks>
        /// Note that entities that are already in the context in some other state will have their state set
        ///             to Added.  Add is a no-op if the entity is already in the context in the Added state.
        /// </remarks>
        public T Add(T entity)
        {
            this.data.Add(entity);
            return entity;
        }

        /// <summary>
        /// Removes an entity.
        /// </summary>
        /// <param name="entity">
        /// The entity to remove.
        /// </param>
        /// <returns>
        /// The removed <see cref="T"/>.
        /// </returns>
        public T Remove(T entity)
        {
            this.data.Remove(entity);
            return entity;
        }

        /// <summary>
        /// Attaches an entity.
        /// </summary>
        /// <param name="entity">The entity to attach.</param>
        /// <returns>The attached entity.</returns>
        public T Attach(T entity)
        {
            this.data.Add(entity);
            return entity;
        }

        /// <summary>
        /// Detaches an entity.
        /// </summary>
        /// <param name="entity">The entity to detach.</param>
        /// <returns>The detached entity.</returns>
        public T Detach(T entity)
        {
            this.data.Remove(entity);
            return entity;
        }

        /// <summary>
        /// Creates a new instance of an entity for the type of this set.
        ///             Note that this instance is NOT added or attached to the set.
        ///             The instance returned will be a proxy if the underlying context is configured to create
        ///             proxies and the entity type meets the requirements for creating a proxy.
        /// </summary>
        /// <returns>
        /// The entity instance, which may be a proxy.
        /// </returns>
        public T Create()
        {
            return Activator.CreateInstance<T>();
        }

        /// <summary>
        /// Creates a new instance of an entity for the type of this set or for a type derived
        ///             from the type of this set.
        ///             Note that this instance is NOT added or attached to the set.
        ///             The instance returned will be a proxy if the underlying context is configured to create
        ///             proxies and the entity type meets the requirements for creating a proxy.
        /// </summary>
        /// <typeparam name="TDerivedEntity">The type of entity to create. </typeparam>
        /// <returns>
        /// The entity instance, which may be a proxy.
        /// </returns>
        public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
        {
            return Activator.CreateInstance<TDerivedEntity>();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.data.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.data.GetEnumerator();
        }

        /// <summary>
        /// The get async enumerator.
        /// </summary>
        /// <returns>
        /// The <see cref="IDbAsyncEnumerator"/>.
        /// </returns>
        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return new AsyncEnumeratorWrapper<T>(this.data.AsEnumerable().GetEnumerator());
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return this.GetAsyncEnumerator();
        }

        private class AsyncQueryProviderWrapper<TEntity> : IDbAsyncQueryProvider
        {
            private readonly IQueryProvider inner;

            internal AsyncQueryProviderWrapper(IQueryProvider inner)
            {
                this.inner = inner;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                return new AsyncEnumerableQuery<TEntity>(expression);
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new AsyncEnumerableQuery<TElement>(expression);
            }

            public object Execute(Expression expression)
            {
                return this.inner.Execute(expression);
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return this.inner.Execute<TResult>(expression);
            }

            public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
            {
                return Task.FromResult(this.Execute(expression));
            }

            public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
            {
                return Task.FromResult(this.Execute<TResult>(expression));
            }
        }

        private class AsyncEnumerableQuery<TEntity> : EnumerableQuery<TEntity>,
                                                      IDbAsyncEnumerable<TEntity>,
                                                      IQueryable<TEntity>
        {
            public AsyncEnumerableQuery(IEnumerable<TEntity> enumerable)
                : base(enumerable)
            {
            }

            public AsyncEnumerableQuery(Expression expression)
                : base(expression)
            {
            }

            IQueryProvider IQueryable.Provider
            {
                get
                {
                    return new AsyncQueryProviderWrapper<TEntity>(this);
                }
            }

            public IDbAsyncEnumerator<TEntity> GetAsyncEnumerator()
            {
                return new AsyncEnumeratorWrapper<TEntity>(this.AsEnumerable().GetEnumerator());
            }

            IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
            {
                return this.GetAsyncEnumerator();
            }
        }

        private class AsyncEnumeratorWrapper<TEntity> : IDbAsyncEnumerator<TEntity>
        {
            private readonly IEnumerator<TEntity> inner;

            public AsyncEnumeratorWrapper(IEnumerator<TEntity> inner)
            {
                this.inner = inner;
            }

            object IDbAsyncEnumerator.Current
            {
                get
                {
                    return this.Current;
                }
            }

            public TEntity Current
            {
                get
                {
                    return this.inner.Current;
                }
            }

            public void Dispose()
            {
                this.inner.Dispose();
            }

            public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
            {
                return Task.FromResult(this.inner.MoveNext());
            }
        }
    }
}