// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPresentationContext.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IPresentationContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation
{
    using System;

    /// <summary>
    /// Context of the presentation containing all relevant information.
    /// </summary>
    public interface IPresentationContext
    {
        /// <summary>
        /// Event that is fired whenever the presentation is being updated,
        /// either because of a generic data change (see <see cref="Generic"/>) or
        /// a time change (see <see cref="Time"/>).
        /// This event will always be fired before updating and will later be
        /// followed by <see cref="Updated"/>.
        /// </summary>
        event EventHandler Updating;

        /// <summary>
        /// Event that is fired when the presentation update process has finished.
        /// This event is always preceded by <see cref="Updating"/>.
        /// Registered listeners can update the <see cref="PresentationUpdatedEventArgs.Updates"/>
        /// to communicate changes to a screen.
        /// </summary>
        event EventHandler<PresentationUpdatedEventArgs> Updated;

        /// <summary>
        /// Gets the config context which can be used to query configuration parameters.
        /// </summary>
        IPresentationConfigContext Config { get; }

        /// <summary>
        /// Gets the generic context which can be used to query generic cell data and
        /// to subscribe to cell changes.
        /// </summary>
        IPresentationGenericContext Generic { get; }

        /// <summary>
        /// Gets the time context which can be used to get the current time and 
        /// to register to be notified when a given time is reached.
        /// </summary>
        IPresentationTimeContext Time { get; }
    }
}