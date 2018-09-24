// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUpdateSink.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IUpdateSink type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Common
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Utility;

    /// <summary>
    /// Interface to be implemented by classes that receive an update
    /// (either an update agent that handles it or an update provider that forwards it).
    /// </summary>
    public interface IUpdateSink : IUpdateComponent
    {
        /// <summary>
        /// Event that is fired when feedback is received from one or more units.
        /// </summary>
        event EventHandler<FeedbackEventArgs> FeedbackReceived;

        /// <summary>
        /// Gets the list of handled units. One of the unit name might be a wildcard
        /// (<see cref="UpdateComponentBase.UnitWildcard"/>) to tell the user of this
        /// class that the sink is interested in all updates.
        /// </summary>
        IEnumerable<string> HandledUnits { get; }

        /// <summary>
        /// Handles the update commands by either handling them locally or forwarding them.
        /// </summary>
        /// <param name="commands">
        /// The update commands.
        /// </param>
        /// <param name="progressMonitor">
        /// The progress monitor that observes the handling of the update command.
        /// </param>
        void HandleCommands(IEnumerable<UpdateCommand> commands, IProgressMonitor progressMonitor);
    }
}