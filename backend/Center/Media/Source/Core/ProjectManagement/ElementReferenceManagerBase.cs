// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementReferenceManagerBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ElementReferenceManagerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ProjectManagement
{
    using System;
    using System.IO;
    using System.Linq;

    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Models;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The element reference manager base.
    /// </summary>
    /// <typeparam name="T">The type of the element.</typeparam>
    public abstract class ElementReferenceManagerBase<T>
    {
        private readonly Lazy<IMediaApplicationState> lazyApplicationState =
            new Lazy<IMediaApplicationState>(() => ServiceLocator.Current.GetInstance<IMediaApplicationState>());

        /// <summary>
        /// Gets the application state.
        /// </summary>
        public IMediaApplicationState ApplicationState
        {
            get
            {
                return this.lazyApplicationState.Value;
            }
        }

        /// <summary>
        /// Sets the references for the given item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public abstract void SetReferences(T item);

        /// <summary>
        /// Unsets the references for the given item.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        public abstract void UnsetReferences(T item);

        /// <summary>
        /// The get resource.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <returns>
        /// The <see cref="ResourceInfoDataViewModel"/>.
        /// </returns>
        protected ResourceInfoDataViewModel GetResource(string filename)
        {
            var resources = this.ApplicationState.CurrentProject.Resources;
            return
                resources.SingleOrDefault(
                    r => Path.GetFileName(r.Filename) == Path.GetFileName(filename) || r.Facename == filename);
        }
    }
}