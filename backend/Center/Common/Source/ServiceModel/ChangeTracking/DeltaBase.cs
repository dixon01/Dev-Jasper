// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeltaBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DeltaBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.ChangeTracking
{
    using System;

    /// <summary>
    /// Defines a base class for delta objects.
    /// </summary>
    public abstract class DeltaBase : ICloneable, IDisposable
    {
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeltaBase"/> class.
        /// </summary>
        /// <param name="deltaOperation">The type of the operation.</param>
        protected DeltaBase(DeltaOperation deltaOperation)
        {
            this.DeltaOperation = deltaOperation;
        }

        /// <summary>
        /// Gets the type of the operation.
        /// </summary>
        public DeltaOperation DeltaOperation { get; private set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        public Version Version { get; protected set; }

        /// <summary>
        /// Gets or sets the date when the object was modified for the last time.
        /// </summary>
        public DateTime? LastModifiedOn { get; protected set; }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            this.Dispose(true);
        }

        /// <summary>
        /// Sets the version to the given value.
        /// </summary>
        /// <param name="version">The version value to be set.</param>
        public void SetVersion(Version version)
        {
            if (version == null)
            {
                throw new ArgumentNullException("version");
            }

            this.Version = version;
        }

        /// <summary>
        /// Sets the last modified on date to the given value.
        /// </summary>
        /// <param name="lastModifiedOn">
        /// The last modified on date to be set.
        /// </param>
        public void SetLastModifiedOn(DateTime? lastModifiedOn)
        {
            this.LastModifiedOn = lastModifiedOn;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        object ICloneable.Clone()
        {
            return this.MemberwiseClone();
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose unmanaged resources here
            }

            this.isDisposed = true;
        }
    }
}