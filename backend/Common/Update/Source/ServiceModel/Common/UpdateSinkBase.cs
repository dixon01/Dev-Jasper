// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateSinkBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateSinkBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Common
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Providers;
    using Gorba.Common.Update.ServiceModel.Utility;

    /// <summary>
    /// Base class for all classes implementing <see cref="IUpdateSink"/>.
    /// </summary>
    public abstract class UpdateSinkBase : UpdateComponentBase, IUpdateSink
    {
        /// <summary>
        /// Event that is fired when feedback is received from one or more units.
        /// </summary>
        public virtual event EventHandler<FeedbackEventArgs> FeedbackReceived;

        /// <summary>
        /// Gets the list of handled units. One of the unit name might be a wildcard
        /// (<see cref="UpdateComponentBase.UnitWildcard"/>) to tell the user of this
        /// class that the sink is interested in all updates.
        /// </summary>
        public abstract IEnumerable<string> HandledUnits { get; }

        /// <summary>
        /// Handles the update commands by forwarding them.
        /// </summary>
        /// <param name="commands">
        ///     The update commands.
        /// </param>
        /// <param name="progressMonitor">
        ///     The progress monitor that observes the upload of the update command.
        /// </param>
        public abstract void HandleCommands(IEnumerable<UpdateCommand> commands, IProgressMonitor progressMonitor);

        /// <summary>
        /// Raises the <see cref="UpdateProviderBase{TConfig}.FeedbackReceived"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseFeedbackReceived(FeedbackEventArgs e)
        {
            var handler = this.FeedbackReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}