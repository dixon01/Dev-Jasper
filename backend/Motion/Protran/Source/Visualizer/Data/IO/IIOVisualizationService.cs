// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIOVisualizationService.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Data.IO
{
    using System;

    using Gorba.Common.Configuration.Protran.IO;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IIOVisualizationService
    {
        /// <summary>
        /// Event fired when the io input is being transformed.
        /// </summary>
        event EventHandler IoInputTransforming;

        /// <summary>
        /// Event that is fired when a telegram successfully passed transformation.
        /// </summary>
        event EventHandler<TransformationChainEventArgs> IoInputTransformed;

        /// <summary>
        /// Event that is fired every time a new Ximple is created
        /// </summary>
        event EventHandler<XimpleEventArgs> XimpleCreated;

        /// <summary>
        /// Gets the generic view dictionary.
        /// </summary>
        Dictionary Dictionary { get; }

        /// <summary>
        /// Gets the IO protocol config.
        /// </summary>
        IOProtocolConfig Config { get; }

        /// <summary>
        /// The send pin changed event.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        /// <param name="pinState">
        /// The pin state.
        /// </param>
        void SendPinChangedEvent(InputHandlingConfig config, bool pinState);
    }
}
