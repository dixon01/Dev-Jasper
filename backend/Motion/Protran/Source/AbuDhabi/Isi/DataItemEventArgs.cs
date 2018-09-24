// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataItemEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataItemEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Isi
{
    using System;

    using Gorba.Common.Protocols.Isi.Messages;

    /// <summary>
    /// Event arguments that encapsulate a <see cref="DataItem"/>
    /// </summary>
    public class DataItemEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataItemEventArgs"/> class.
        /// </summary>
        /// <param name="dataItem">
        /// The data item.
        /// </param>
        public DataItemEventArgs(DataItem dataItem)
        {
            this.DataItem = dataItem;
        }

        /// <summary>
        /// Gets the data item.
        /// </summary>
        public DataItem DataItem { get; private set; }
    }
}
