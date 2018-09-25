// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAbuDhabiVisualizationService.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Visualization service interface for Abu Dhabi protocol.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Data.AbuDhabi
{
    using System;

    using Gorba.Common.Protocols.Isi.Messages;
    using Gorba.Motion.Protran.AbuDhabi;
    using Gorba.Motion.Protran.AbuDhabi.Isi;

    /// <summary>
    /// Visualization service interface for Abu Dhabi protocol.
    /// </summary>
    public interface IAbuDhabiVisualizationService
    {
        /// <summary>
        /// Event that is fired whenever an ISI message was sent out from Protran.
        /// </summary>
        event EventHandler<IsiMessageEventArgs> IsiMessageSent;

        /// <summary>
        /// Event that is fired whenever an ISI message has been enqueued in Protran.
        /// </summary>
        event EventHandler<IsiMessageEventArgs> IsiMessageEnqueued;

        /// <summary>
        /// Event that is fired whenever an ISI data item has been transformed.
        /// </summary>
        event EventHandler<DataItemTransformationEventArgs> DataItemTransformed;

        /// <summary>
        /// Enqueues a new ISI message like it were received from the OBU.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        void EnqueueIsiMessage(IsiMessageBase message);
    }
}
