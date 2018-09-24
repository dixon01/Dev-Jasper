// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataViewModelManager{TDataViewModel,TEntity}.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataViewModelManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.DataViewModels
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Cache to get <typeparamref name="TDataViewModel, TEntity"/> data view models and keep them synchronized
    /// with the corresponding <typeparamref name="TEntity"/> object.
    /// </summary>
    /// <typeparam name="TDataViewModel">The type of the data view model.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public abstract class DataViewModelManager<TDataViewModel, TEntity>
        where TEntity : class
    {
        private static readonly Lazy<DataViewModelManager<TDataViewModel, TEntity>> DefaultInstance =
            new Lazy<DataViewModelManager<TDataViewModel, TEntity>>(CreateDefaultInstance);

        static DataViewModelManager()
        {
            ResetCurrent();
        }

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        public static DataViewModelManager<TDataViewModel, TEntity> Current { get; private set; }

        /// <summary>
        /// Resets the current instance.
        /// </summary>
        public static void ResetCurrent()
        {
            SetCurrent(DefaultInstance.Value);
        }

        /// <summary>
        /// Sets the current instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public static void SetCurrent(DataViewModelManager<TDataViewModel, TEntity> instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            Current = instance;
        }

        /// <summary>
        /// Gets the <typeparamref name="TDataViewModel"/> corresponding to the specified <paramref name="entity"/>.
        /// The view model is updated if found in the internal cache (key used: Id); otherwise, it is created
        /// and added to the cache.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The entity data view model corresponding to the specified entity according to its Id.</returns>
        public abstract TDataViewModel Get(TEntity entity);

        private static DataViewModelManager<TDataViewModel, TEntity> CreateDefaultInstance()
        {
            return DefaultDataViewModelManager.Instance;
        }

        private class DefaultDataViewModelManager : DataViewModelManager<TDataViewModel, TEntity>
        {
            private readonly Dictionary<int, WeakReference> dataViewModels =
                new Dictionary<int, WeakReference>();

            static DefaultDataViewModelManager()
            {
                Instance = new DefaultDataViewModelManager();
            }

            public static DefaultDataViewModelManager Instance { get; private set; }

            public override TDataViewModel Get(TEntity entity)
            {
                throw new NotSupportedException(
                    "The DataViewModelManager for the given type has not been initialized to a valid instance");
            }
        }
    }
}
