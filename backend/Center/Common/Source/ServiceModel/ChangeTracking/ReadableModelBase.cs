// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadableModelBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ReadableModelBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.ChangeTracking
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines a base for readable objects.
    /// Subclasses should always inherit from <see cref="ReadableModelBase{TDelta}"/>,
    /// not from this class directly.
    /// </summary>
    public abstract class ReadableModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadableModelBase"/> class.
        /// </summary>
        protected ReadableModelBase()
        {
            this.Version = new Version(0);
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the date when model was created.
        /// </summary>
        public DateTime CreatedOn { get; protected set; }

        /// <summary>
        /// Gets or sets the version of the model.
        /// </summary>
        public Version Version { get; protected set; }

        /// <summary>
        /// Gets or sets the date when the model was modified.
        /// </summary>
        public DateTime? LastModifiedOn { get; protected set; }

        /// <summary>
        /// Loads reference properties.
        /// </summary>
        /// <returns>A <see cref="Task"/> that can be awaited.</returns>
        public virtual Task LoadReferencePropertiesAsync()
        {
            // TODO: make abstract when the class can be abstract
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads navigation properties (references and collections).
        /// </summary>
        /// <returns>A <see cref="Task"/> that can be awaited.</returns>
        public virtual Task LoadNavigationPropertiesAsync()
        {
            // TODO: make abstract when the class can be abstract
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads XML properties (usually with large contents).
        /// </summary>
        /// <returns>A <see cref="Task"/> that can be awaited.</returns>
        public virtual Task LoadXmlPropertiesAsync()
        {
            // TODO: make abstract when the class can be abstract
            throw new NotImplementedException();
        }

        /// <summary>
        /// raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the changed property.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = this.PropertyChanged;
            if (handler == null)
            {
                return;
            }

            handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}