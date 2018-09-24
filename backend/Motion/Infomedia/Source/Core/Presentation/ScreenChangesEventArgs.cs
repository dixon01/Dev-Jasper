// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenChangesEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ScreenChangesEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation
{
    using System;
    using System.Collections.Generic;

    using Gorba.Motion.Infomedia.Entities.Messages;

    /// <summary>
    /// The screen changes event args.
    /// </summary>
    public class ScreenChangesEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenChangesEventArgs"/> class.
        /// </summary>
        public ScreenChangesEventArgs()
        {
            this.Changes = new List<ScreenChange>();
            this.Updates = new List<ScreenUpdate>();
        }

        /// <summary>
        /// Gets or sets the changes communicated with this object.
        /// </summary>
        public List<ScreenChange> Changes { get; set; }

        /// <summary>
        /// Gets or sets the updates to existing screen items communicated
        /// with this object.
        /// </summary>
        public List<ScreenUpdate> Updates { get; set; }
    }
}