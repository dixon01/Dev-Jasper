// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExpectedUpdateGroupDeltaNotification.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The expected update group delta notification.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.IntegrationTests.Notifications
{
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Common.Utility.IntegrationTests;

    /// <summary>
    /// The expected update group delta notification.
    /// </summary>
    public class ExpectedUpdateGroupDeltaNotification : ExpectationBase
    {
        /// <summary>
        /// Gets or sets the expected notification.
        /// </summary>
        public UpdateGroupDeltaNotification ExpectedNotification { get; set; }

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
            var delta = notification as UpdateGroupDeltaNotification;

            if (delta == null)
            {
                context.Fail("Notification is not of the expected type.");
                return false;
            }

            return true;
        }
    }
}
