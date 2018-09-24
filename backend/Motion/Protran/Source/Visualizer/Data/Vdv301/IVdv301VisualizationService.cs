// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IVdv301VisualizationService.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IVdv301VisualizationService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Data.Vdv301
{
    using System;

    using Gorba.Common.Protocols.Vdv301.Services;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;

    /// <summary>
    /// The visualization service interface used by controls of VDV 301 visualization.
    /// </summary>
    public interface IVdv301VisualizationService
    {
        /// <summary>
        /// Event that is fired every time new data arrives in one of the service handlers.
        /// </summary>
        event EventHandler<DataUpdateEventArgs<object>> DataUpdated;

        /// <summary>
        /// Event that is fired when a telegram successfully passed transformation.
        /// </summary>
        event EventHandler<TransformationChainEventArgs> ElementTransformed;

        /// <summary>
        /// Event that is fired every time a new Ximple is created by the protocol.
        /// </summary>
        event EventHandler<XimpleEventArgs> XimpleCreated;

        /// <summary>
        /// Gets the IBIS service locator for this service.
        /// </summary>
        VisualizerIbisServiceLocator IbisServiceLocator { get; }

        /// <summary>
        /// Gets the generic view dictionary.
        /// </summary>
        Dictionary Dictionary { get; }
    }
}