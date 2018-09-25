// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateStateInfoAck.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateStateInfoAck type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.AzureClient
{
    using Gorba.Common.Update.ServiceModel.Messages;

    /// <summary>
    /// The acknowledge for update state info messages.
    /// </summary>
    public class UpdateStateInfoAck
    {
        /// <summary>
        /// Gets or sets the unit id.
        /// </summary>
        public UnitId UnitId { get; set; }

        /// <summary>
        /// Gets or sets the update id.
        /// </summary>
        public UpdateId UpdateId { get; set; }

        /// <summary>
        /// Gets or sets the update state.
        /// </summary>
        public UpdateState UpdateState { get; set; }
    }
}