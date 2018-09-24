// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityCollection.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EntityCollection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels
{
    using System.Collections;
    using System.Collections.ObjectModel;

    using Gorba.Center.Common.Wpf.Framework.Controllers;

    /// <summary>
    /// Concrete implementation of <see cref="EntityCollectionBase"/> for a given type of items.
    /// </summary>
    /// <typeparam name="T">
    /// The type of items stored in this collection.
    /// </typeparam>
    public class EntityCollection<T> : EntityCollectionBase
        where T : ReadOnlyDataViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityCollection{T}"/> class.
        /// </summary>
        /// <param name="displayName">
        /// The display name of this collection (i.e. the property name of the property it belongs to).
        /// </param>
        /// <param name="entityName">
        /// The entity name (the name of the <see cref="T"/> entity).
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry to get commands from.
        /// </param>
        public EntityCollection(string displayName, string entityName, ICommandRegistry commandRegistry)
            : base(displayName, entityName, commandRegistry)
        {
            this.Items = new ObservableCollection<T>();
        }

        /// <summary>
        /// Gets the typed items stored.
        /// </summary>
        public ObservableCollection<T> Items { get; private set; }

        /// <summary>
        /// Gets the collection entities.
        /// </summary>
        public override ICollection Entities
        {
            get
            {
                return this.Items;
            }
        }
    }
}