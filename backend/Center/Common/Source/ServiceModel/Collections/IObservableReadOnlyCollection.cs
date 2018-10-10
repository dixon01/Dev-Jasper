// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IObservableReadOnlyCollection.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IObservableReadOnlyCollection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Collections
{
    using System.Collections.Generic;
    using System.Collections.Specialized;

    /// <summary>
    /// A read-only list that supports collection change notifications.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the items in this list.
    /// </typeparam>
    public interface IObservableReadOnlyCollection<out T> : IReadOnlyList<T>, INotifyCollectionChanged
    {
    }
}