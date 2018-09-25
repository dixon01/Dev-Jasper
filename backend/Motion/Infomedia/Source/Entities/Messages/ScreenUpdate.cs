// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScreenUpdate.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ScreenUpdate type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Entities.Messages
{
    using System.Collections.Generic;

    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// Update message of several properties of a screen.
    /// </summary>
    public class ScreenUpdate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScreenUpdate"/> class.
        /// </summary>
        public ScreenUpdate()
        {
            this.Updates = new List<ItemUpdate>();
        }

        /// <summary>
        /// Gets or sets the affected <see cref="RootItem"/>'s <see cref="ItemBase.Id"/>.
        /// </summary>
        public int RootId { get; set; }

        /// <summary>
        /// Gets or sets the list of all property updates.
        /// </summary>
        public List<ItemUpdate> Updates { get; set; }
    }
}