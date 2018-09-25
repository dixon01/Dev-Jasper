// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TableEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Data
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Protocols.Ximple;

    /// <summary>
    /// Event args for DataReceived events.
    /// </summary>
    public class TableEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TableEventArgs"/> class.
        /// </summary>
        /// <param name="newValues">The list of updated cells.</param>
        public TableEventArgs(List<XimpleCell> newValues)
        {
            this.NewValues = newValues;
        }

        /// <summary>
        /// Gets the cells that have been updated.
        /// </summary>
        public List<XimpleCell> NewValues { get; private set; }
    }
}
