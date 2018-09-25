// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.DataViewModels;
    using Gorba.Center.Admin.Core.Models;
    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Common.Utility.Core;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// The base class for all data controllers.
    /// </summary>
    public abstract class DataControllerBase : SynchronizableControllerBase
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly Logger Logger;

        private readonly List<string> detailsInitializedIds = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DataControllerBase"/> class.
        /// </summary>
        /// <param name="dataController">
        /// The data controller that created this controller.
        /// </param>
        protected DataControllerBase(DataController dataController)
        {
            this.Logger = LogHelper.GetLogger(this.GetType());
            this.DataController = dataController;
        }

        /// <summary>
        /// Gets or sets the data controller that created this controller.
        /// </summary>
        public DataController DataController { get; set; }

        /// <summary>
        /// Gets the data view model factory.
        /// </summary>
        public DataViewModelFactory Factory
        {
            get
            {
                return this.DataController.Factory;
            }
        }

        /// <summary>
        /// Gets the connection controller.
        /// </summary>
        public IConnectionController ConnectionController { get; private set; }

        /// <summary>
        /// Gets the application state.
        /// </summary>
        public IAdminApplicationState ApplicationState { get; private set; }

        /// <summary>
        /// Initializes this controller with the given <see cref="ConnectionController"/>.
        /// </summary>
        /// <param name="connectionController">
        /// The connection controller.
        /// </param>
        public virtual void Initialize(IConnectionController connectionController)
        {
            this.ConnectionController = connectionController;

            if (this.ApplicationState == null)
            {
                this.ApplicationState = ServiceLocator.Current.GetInstance<IAdminApplicationState>();
                this.ApplicationState.PropertyChanged += this.ApplicationStateOnPropertyChanged;
            }
        }

        /// <summary>
        /// Asynchronous method to wait on this controller until it finished loading all data.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> to wait on.
        /// </returns>
        public abstract Task AwaitAllDataAsync();

        /// <summary>
        /// Checks if this controller supports the given entity instance.
        /// </summary>
        /// <param name="dataViewModel">
        /// The data view model of the entity instance.
        /// </param>
        /// <returns>
        /// True if this controller supports the given entity.
        /// </returns>
        public abstract bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel);

        /// <summary>
        /// Asynchronously gets the entity with the given id string.
        /// </summary>
        /// <param name="idString">
        /// The id string.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to wait for the data view model with the given id.
        /// The returned view model can be null if no entity with the given id can be found.
        /// </returns>
        public abstract Task<ReadOnlyDataViewModelBase> GetEntityAsync(string idString);

        /// <summary>
        /// Creates a new <see cref="DataViewModelBase"/> object for the type of entity this controller handles.
        /// </summary>
        /// <returns>
        /// The newly created <see cref="DataViewModelBase"/> implementation.
        /// </returns>
        public abstract Task<DataViewModelBase> CreateEntityAsync();

        /// <summary>
        /// Creates a <see cref="DataViewModelBase"/> for the given <paramref name="dataViewModel"/>.
        /// </summary>
        /// <param name="dataViewModel">
        /// The data view model.
        /// </param>
        /// <returns>
        /// The newly created <see cref="DataViewModelBase"/> implementation.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="dataViewModel"/> is not of the type this controller handles.
        /// </exception>
        public abstract Task<DataViewModelBase> EditEntityAsync(ReadOnlyDataViewModelBase dataViewModel);

        /// <summary>
        /// Creates a new <see cref="DataViewModelBase"/> with a copy of the given <paramref name="dataViewModel"/>.
        /// </summary>
        /// <param name="dataViewModel">
        /// The data view model.
        /// </param>
        /// <returns>
        /// The newly created <see cref="DataViewModelBase"/> implementation.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="dataViewModel"/> is not of the type this controller handles.
        /// </exception>
        public abstract Task<DataViewModelBase> CopyEntityAsync(ReadOnlyDataViewModelBase dataViewModel);

        /// <summary>
        /// Asynchronously saves the given entity to the background system.
        /// </summary>
        /// <param name="dataViewModel">
        /// The data view model to be saved.
        /// This is usually an object returned by <see cref="EditEntity"/>.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> with the saved version of the view model to wait on.
        /// </returns>
        public abstract Task<ReadOnlyDataViewModelBase> SaveEntityAsync(DataViewModelBase dataViewModel);

        /// <summary>
        /// Asynchronously deletes an entity in the background system.
        /// </summary>
        /// <param name="dataViewModel">
        /// The data view model to delete.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to wait on.
        /// </returns>
        public abstract Task DeleteEntityAsync(ReadOnlyDataViewModelBase dataViewModel);

        /// <summary>
        /// Asynchronously loads the details of the given data view model.
        /// </summary>
        /// <param name="dataViewModel">
        /// The data view model.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to await for completion.
        /// </returns>
        public async Task LoadEntityDetailsAsync(ReadOnlyDataViewModelBase dataViewModel)
        {
            if (this.detailsInitializedIds.Contains(dataViewModel.GetIdString()))
            {
                return;
            }

            this.detailsInitializedIds.Add(dataViewModel.GetIdString());
            await this.DoLoadEntityDetailsAsync(dataViewModel);
        }

        /// <summary>
        /// Starts reloading all data.
        /// </summary>
        /// <param name="force">
        /// A flag indicating if the data should be reloaded even if it wasn't loaded yet at all.
        /// </param>
        protected abstract void StartReloadAllData(bool force);

        /// <summary>
        /// Implementation of the asynchronous load of the details of the given data view model.
        /// </summary>
        /// <param name="dataViewModel">
        /// The data view model.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to await for completion.
        /// </returns>
        protected abstract Task DoLoadEntityDetailsAsync(ReadOnlyDataViewModelBase dataViewModel);

        private void ApplicationStateOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != "CurrentTenant")
            {
                return;
            }

            this.detailsInitializedIds.Clear();
            if (this.ApplicationState.CurrentTenant == null)
            {
                return;
            }

            this.StartReloadAllData(false);
        }
    }
}