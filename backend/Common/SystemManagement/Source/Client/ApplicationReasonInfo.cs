// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationReasonInfo.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationReasonInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Client
{
    using System;

    using Gorba.Common.SystemManagement.ServiceModel;

    /// <summary>
    /// Information why an application was launched or exited.
    /// </summary>
    public class ApplicationReasonInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationReasonInfo"/> class.
        /// </summary>
        /// <param name="reason">
        /// The reason.
        /// </param>
        /// <param name="explanation">
        /// The explanation as a user readable string.
        /// </param>
        /// <param name="timestampUtc">
        /// The the event time in UTC time.
        /// </param>
        public ApplicationReasonInfo(ApplicationReason reason, string explanation, DateTime timestampUtc)
        {
            this.Reason = reason;
            this.Explanation = explanation;
            this.TimestampUtc = timestampUtc;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationReasonInfo"/> class from the given
        /// <see cref="Motion.SystemManager.ServiceModel.Messages.ApplicationReasonInfo"/> message object.
        /// </summary>
        /// <param name="reasonInfo">
        /// The <see cref="Motion.SystemManager.ServiceModel.Messages.ApplicationReasonInfo"/> object.
        /// </param>
        internal ApplicationReasonInfo(Motion.SystemManager.ServiceModel.Messages.ApplicationReasonInfo reasonInfo)
        {
            this.Reason = MessageConverter.Convert(reasonInfo.Reason);
            this.Explanation = reasonInfo.Explanation;
            this.TimestampUtc = reasonInfo.TimestampUtc;
        }

        /// <summary>
        /// Gets the reason.
        /// </summary>
        public ApplicationReason Reason { get; private set; }

        /// <summary>
        /// Gets the explanation as a user readable string.
        /// </summary>
        public string Explanation { get; private set; }

        /// <summary>
        /// Gets the event time in UTC time.
        /// </summary>
        public DateTime TimestampUtc { get; private set; }

        /// <summary>
        /// Converts this object to an <see cref="Motion.SystemManager.ServiceModel.Messages.ApplicationReasonInfo"/>
        /// message object
        /// </summary>
        /// <returns>
        /// The <see cref="Motion.SystemManager.ServiceModel.Messages.ApplicationReasonInfo"/> object.
        /// </returns>
        internal Motion.SystemManager.ServiceModel.Messages.ApplicationReasonInfo ToMessage()
        {
            return new Motion.SystemManager.ServiceModel.Messages.ApplicationReasonInfo
                       {
                           Reason = MessageConverter.Convert(this.Reason),
                           Explanation = this.Explanation,
                           TimestampUtc = this.TimestampUtc
                       };
        }
    }
}