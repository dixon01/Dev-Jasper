// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PresentationUpdatedEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PresentationUpdatedEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation
{
    using System;
    using System.Collections.Generic;

    using Gorba.Motion.Infomedia.Entities.Messages;

    /// <summary>
    /// Event arguments for the <see cref="IPresentationContext.Updated"/> event.
    /// </summary>
    public class PresentationUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PresentationUpdatedEventArgs"/> class.
        /// </summary>
        public PresentationUpdatedEventArgs()
        {
            this.Updates = new List<ScreenUpdate>();
        }

        /// <summary>
        /// Gets the updates. Listeners getting this
        /// event can add updates to this list and they will
        /// be communicated by the <see cref="PresentationManager"/>
        /// to all subscribed renderers.
        /// </summary>
        public List<ScreenUpdate> Updates { get; private set; }
    }
}