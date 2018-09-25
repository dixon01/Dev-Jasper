// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReadOnlyEntityCollection.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IReadOnlyEntityCollection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    using Gorba.Center.Admin.Core.DataViewModels;

    // ReSharper disable once PossibleInterfaceMemberAmbiguity

    /// <summary>
    /// Interface for a collection of entity objects that can only be read.
    /// </summary>
    /// <typeparam name="T">
    /// The type of entity in this collection.
    /// </typeparam>
    public interface IReadOnlyEntityCollection<out T>
        : IReadOnlyList<T>, IList, INotifyCollectionChanged, INotifyPropertyChanged
        where T : ReadOnlyDataViewModelBase
    {
        /// <summary>
        /// Gets a value indicating whether this collection is currently loading its contents.
        /// </summary>
        bool IsLoading { get; }

        /// <summary>
        /// Gets the number of items in this list.
        /// </summary>
        new int Count { get; }
    }
}