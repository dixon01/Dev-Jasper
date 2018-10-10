// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpectationBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ExpectationBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.IntegrationTests
{
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Common.Utility.IntegrationTests;

    /// <summary>
    /// The expectation base.
    /// </summary>
    public abstract class ExpectationBase
    {
        /// <summary>
        /// Gets or sets the id of the expectation.
        /// </summary>
        /// <value>
        /// The id.
        /// </value>
        public int ExpectationId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to omit the entity id while checking the notification.
        /// </summary>
        public bool OmitEntityId { get; set; }

       /// <summary>
        /// Verifies if the given <paramref name="notification"/> is equivalent to this expected one.
        /// </summary>
        /// <param name="context">The test context.</param>
        /// <param name="notification">The notification.</param>
        /// <returns>
        ///   <c>true</c> if the received notification is equivalent to the expected one; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool Match(IntegrationTestContext context, Notification notification);
    }
}