// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReferenceChange{T}.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ReferenceChange type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.ChangeTracking
{
    /// <summary>
    /// Defines a changed reference.
    /// </summary>
    /// <typeparam name="T">The type of the identifier.</typeparam>
    public class ReferenceChange<T>
        where T : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceChange{T}"/> class.
        /// </summary>
        /// <param name="originalReferenceId">The original value id.</param>
        public ReferenceChange(T? originalReferenceId)
        {
            this.OriginalReferenceId = originalReferenceId;
        }

        /// <summary>
        /// Gets the original reference identifier.
        /// </summary>
        public T? OriginalReferenceId { get; private set; }

        /// <summary>
        /// Gets the changed reference identifier.
        /// </summary>
        public T? ReferenceId { get; private set; }

        /// <summary>
        /// Changes the reference.
        /// </summary>
        /// <param name="referenceId">The new reference identifier.</param>
        /// <returns>This reference change object with the updated reference identifier.</returns>
        public ReferenceChange<T> ChangeReference(T? referenceId)
        {
            this.ReferenceId = referenceId;
            return this;
        }
    }
}