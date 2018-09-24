// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PartControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PartControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Common.Configuration.HardwareDescription;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Base class for all controllers for a unit configuration part.
    /// </summary>
    public abstract class PartControllerBase : SynchronizableControllerBase
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly Logger Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartControllerBase"/> class.
        /// </summary>
        /// <param name="key">
        /// The unique key to identify this part.
        /// </param>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        protected PartControllerBase(string key, CategoryControllerBase parent)
        {
            this.Logger = LogHelper.GetLogger(this.GetType());

            this.Key = key;
            this.Parent = parent;
        }

        /// <summary>
        /// Event that is fired whenever any of the properties of the view model change.
        /// </summary>
        public event EventHandler ViewModelUpdated;

        /// <summary>
        /// Gets the unique key to identify this part.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Gets the parent controller.
        /// </summary>
        public CategoryControllerBase Parent { get; private set; }

        /// <summary>
        /// Asynchronously prepares this part controller with the given hardware descriptor.
        /// </summary>
        /// <param name="descriptor">
        /// The hardware descriptor.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to wait on.
        /// </returns>
        public virtual Task PrepareAsync(HardwareDescriptor descriptor)
        {
            this.Prepare(descriptor);
            return Task.FromResult(0);
        }

        /// <summary>
        /// Initializes this controller and creates the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="PartViewModelBase"/> implementation controlled by this controller.
        /// </returns>
        public abstract PartViewModelBase Initialize();

        /// <summary>
        /// Loads the unit config data into this part.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public abstract void Load(UnitConfigPart partData);

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public abstract void Save(UnitConfigPart partData);

        /// <summary>
        /// Prepares this part controller with the given hardware descriptor.
        /// </summary>
        /// <param name="descriptor">
        /// The hardware descriptor.
        /// </param>
        protected virtual void Prepare(HardwareDescriptor descriptor)
        {
        }

        /// <summary>
        /// Helper method to get a part controller with the given unique key.
        /// </summary>
        /// <param name="key">
        /// The the unique key to identify the searched part.
        /// If this is null, the first part of the given type is returned.
        /// </param>
        /// <typeparam name="T">
        /// The type of part expected.
        /// </typeparam>
        /// <returns>
        /// The part.
        /// </returns>
        /// <exception cref="KeyNotFoundException">
        /// If the given key and/or type don't exist.
        /// </exception>
        protected T GetPart<T>(string key = null) where T : PartControllerBase
        {
            return this.Parent.Parent.GetPart<T>(key);
        }

        /// <summary>
        /// Raises the <see cref="ViewModelUpdated"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseViewModelUpdated(EventArgs e)
        {
            var handler = this.ViewModelUpdated;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}