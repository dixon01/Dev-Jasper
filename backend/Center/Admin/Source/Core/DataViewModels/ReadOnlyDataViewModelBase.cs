// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadOnlyDataViewModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ReadOnlyDataViewModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.DataViewModels
{
    using Gorba.Center.Common.Wpf.Framework.ViewModels;

    /// <summary>
    /// The base class for read-only entity data view models.
    /// </summary>
    public abstract class ReadOnlyDataViewModelBase : SynchronizableViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyDataViewModelBase"/> class.
        /// </summary>
        /// <param name="factory">
        /// The data view model factory.
        /// </param>
        protected ReadOnlyDataViewModelBase(DataViewModelFactory factory)
        {
            this.Factory = factory;
        }

        /// <summary>
        /// Gets the data view model factory.
        /// </summary>
        public DataViewModelFactory Factory { get; private set; }

        /// <summary>
        /// Gets the display text of this data view model.
        /// </summary>
        public abstract string DisplayText { get; }

        /// <summary>
        /// Gets the string representation of the id.
        /// </summary>
        /// <returns>
        /// The id as a <see cref="string"/>.
        /// </returns>
        public abstract string GetIdString();

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return this.DisplayText;
        }
    }
}