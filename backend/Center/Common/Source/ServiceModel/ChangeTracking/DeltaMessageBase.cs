// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DeltaMessageBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DeltaMessageBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.ChangeTracking
{
    using System;

    /// <summary>
    /// Base class for all notification messages with a delta.
    /// </summary>
    public abstract class DeltaMessageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeltaMessageBase"/> class.
        /// </summary>
        protected DeltaMessageBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeltaMessageBase"/> class.
        /// </summary>
        /// <param name="delta">
        /// The delta for which this message is created.
        /// </param>
        protected DeltaMessageBase(DeltaBase delta)
        {
            this.DeltaOperation = delta.DeltaOperation;
            if (delta.Version != null)
            {
                this.Version = delta.Version.Value;
            }

            this.LastModifiedOn = delta.LastModifiedOn;
        }

        /// <summary>
        /// Gets or sets the type of the operation.
        /// </summary>
        public DeltaOperation DeltaOperation { get; set; }

        /// <summary>
        /// Gets or sets the version of the delta.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the date when the object was modified for the last time.
        /// </summary>
        public DateTime? LastModifiedOn { get; set; }

        /// <summary>
        /// Fills the given <paramref name="delta"/> with the properties from this base class.
        /// </summary>
        /// <param name="delta">
        /// The delta.
        /// </param>
        protected void FillDelta(DeltaBase delta)
        {
            delta.SetVersion(new Version(this.Version));
            if (this.LastModifiedOn.HasValue)
            {
                delta.SetLastModifiedOn(this.LastModifiedOn);
            }
        }

        /// <summary>
        /// A User Defined Property name-value pair.
        /// </summary>
        public class Property
        {
            /// <summary>
            /// Gets or sets the name of the property.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the value of the property.
            /// </summary>
            public string Value { get; set; }
        }
    }
}
