// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IObservableReadOnlyDictionary{TKey,TValue}.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IObservableReadOnlyDictionary type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Collections
{
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;

    /// <summary>
    /// A read-only dictionary that supports collection change notifications.
    /// </summary>
    /// <typeparam name="TKey">
    /// The type of the items in this dictionary.
    /// </typeparam>
    /// <typeparam name="TValue">
    /// The type of the values in this dictionary.
    /// </typeparam>
    /// <remarks>
    /// <see cref="INotifyPropertyChanged.PropertyChanged"/> events are fired when an item is added or removed.
    /// There will be one event for each of the following property names:
    /// - Count
    /// - Item[]
    /// - Keys
    /// - Values
    /// The <see cref="INotifyCollectionChanged.CollectionChanged"/> event is fired if a key is added or removed.
    /// </remarks>
    public interface IObservableReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>,
                                                                   INotifyCollectionChanged,
                                                                   INotifyPropertyChanged
    {
    }
}