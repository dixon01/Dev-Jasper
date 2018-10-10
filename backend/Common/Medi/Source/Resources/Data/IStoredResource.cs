// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStoredResource.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IStoredResource type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Resources.Data
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Resources;

    /// <summary>
    /// Interface to a resource stored in a <see cref="IResourceDataStore"/>.
    /// </summary>
    public interface IStoredResource
    {
        /// <summary>
        /// Event that is fired whenever the <see cref="StoredResource.State"/> changes.
        /// </summary>
        event EventHandler StateChanged;

        /// <summary>
        /// Gets the unique resource id.
        /// </summary>
        ResourceId Id { get; }

        /// <summary>
        /// Gets or sets the state of this resource.
        /// </summary>
        ResourceState State { get; set; }

        /// <summary>
        /// Gets or sets the original source address where the
        /// resource comes from.
        /// </summary>
        MediAddress Source { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this resource is only temporary
        /// and will be deleted once sent to the requested destination.
        /// </summary>
        bool IsTemporary { get; set; }

        /// <summary>
        /// Gets or sets the size of the resource.
        /// </summary>
        long Size { get; set; }

        /// <summary>
        /// Gets the list of currently ongoing resource transfers.
        /// </summary>
        List<ResourceTransfer> Transfers { get; }

        /// <summary>
        /// Gets the list of references.
        /// </summary>
        List<string> References { get; }

        /// <summary>
        /// Gets or sets the store reference.
        /// This can be null if the resource is not yet available
        /// or the resource is "checked out".
        /// </summary>
        string StoreReference { get; set; }

        /// <summary>
        /// Gets or sets the original file name.
        /// </summary>
        string OriginalFileName { get; set; }

        /// <summary>
        /// Gets or sets the set index.
        /// </summary>
        int SetIndex { get; set; }
    }
}