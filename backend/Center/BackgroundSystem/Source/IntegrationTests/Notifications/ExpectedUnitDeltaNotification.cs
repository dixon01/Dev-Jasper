// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpectedUnitDeltaNotification.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The expected unit delta notification.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.IntegrationTests.Notifications
{
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
    using Gorba.Common.Utility.IntegrationTests;

    /// <summary>
    /// The expected unit delta notification.
    /// </summary>
    public class ExpectedUnitDeltaNotification : ExpectationBase
    {
        /// <summary>
        /// Gets or sets the expected notification.
        /// </summary>
        public UnitDeltaNotification ExpectedNotification { get; set; }

        /// <summary>
        /// Verifies if the given <paramref name="notification"/> is equivalent to this expected one.
        /// </summary>
        /// <param name="context">The test context.</param>
        /// <param name="notification">The notification.</param>
        /// <returns>
        ///   <c>true</c> if the received notification is equivalent to the expected one; otherwise, <c>false</c>.
        /// </returns>
        public override bool Match(IntegrationTestContext context, Notification notification)
        {
            var delta = notification as UnitDeltaNotification;

            if (delta == null)
            {
                context.Fail("Notification is not of the expected type.");
                return false;
            }

            var entityIdCheck = this.OmitEntityId || this.ExpectedNotification.EntityId == delta.EntityId;
            return this.ExpectedNotification.WasAccepted == delta.WasAccepted
                && entityIdCheck
                && this.ExpectedNotification.NotificationType == delta.NotificationType;
        }
    }
}
