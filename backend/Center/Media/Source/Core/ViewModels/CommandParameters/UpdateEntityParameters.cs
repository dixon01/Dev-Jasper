// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateEntityParameters.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels.CommandParameters
{
    using System.Collections.Generic;

    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;

    /// <summary>
    /// Defines the parameters needed to undo / redo a property change.
    /// </summary>
    public class UpdateEntityParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateEntityParameters"/> class.
        /// </summary>
        /// <param name="oldElements">
        /// The old elements.
        /// </param>
        /// <param name="newElements">
        /// The new (updated) elements.
        /// </param>
        /// <param name="elementsContainerReference">
        /// the elements container
        /// </param>
        /// <param name="newResolution">
        /// The new Resolution.
        /// </param>
        public UpdateEntityParameters(
            IEnumerable<DataViewModelBase> oldElements,
            IEnumerable<DataViewModelBase> newElements,
            IEnumerable<DataViewModelBase> elementsContainerReference,
            ResolutionConfigDataViewModel newResolution = null)
        {
            this.OldElements = oldElements;
            this.NewElements = newElements;
            this.ElementsContainerReference = elementsContainerReference;
            this.NewResolution = newResolution;
        }

        /// <summary>
        /// Gets the new (updated) elements.
        /// </summary>
        public IEnumerable<DataViewModelBase> NewElements { get; private set; }

        /// <summary>
        /// Gets the old elements.
        /// </summary>
        public IEnumerable<DataViewModelBase> OldElements { get; private set; }

        /// <summary>
        /// Gets the old elements.
        /// </summary>
        public IEnumerable<DataViewModelBase> ElementsContainerReference { get; private set; }

        /// <summary>
        /// Gets the new resolution.
        /// </summary>
        public ResolutionConfigDataViewModel NewResolution { get; private set; }
    }
}
