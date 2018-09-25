// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlyEntityCollection.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ReadOnlyEntityCollection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using Gorba.Center.Admin.Core.DataViewModels;

    /// <summary>
    /// Implementation of <see cref="IReadOnlyEntityCollection{T}"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of entity stored in this collection.
    /// </typeparam>
    public class ReadOnlyEntityCollection<T> : ReadOnlyObservableCollection<T>, IReadOnlyEntityCollection<T>
        where T : ReadOnlyDataViewModelBase
    {
        private bool isLoading;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyEntityCollection{T}"/> class.
        /// </summary>
        /// <param name="list">
        /// The list on which this collection is based.
        /// </param>
        public ReadOnlyEntityCollection(ObservableCollection<T> list)
            : base(list)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether this collection is currently loading its contents.
        /// </summary>
        public bool IsLoading
        {
            get
            {
                return this.isLoading;
            }

            set
            {
                if (this.isLoading == value)
                {
                    return;
                }

                this.isLoading = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("IsLoading"));
            }
        }
    }
}