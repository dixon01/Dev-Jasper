// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenChanges.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ScreenChanges type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Entities.Messages
{
    using System.Collections.Generic;

    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// Message that notifies a renderer that a new screen should be shown.
    /// </summary>
    public class ScreenChanges
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenChanges"/> class.
        /// </summary>
        public ScreenChanges()
        {
            this.Changes = new List<ScreenChange>();
            this.Updates = new List<ScreenUpdate>();
        }

        public ScreenChanges(List<ScreenChange> changes, List<ScreenUpdate> updates = null)
        {
            this.Changes = changes;
            if (updates != null)
            {
                this.Updates = updates;
            }
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
