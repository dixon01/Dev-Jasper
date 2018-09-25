// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XimpleSourceBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the XimpleSourceBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi
{
    using System;
    using System.Globalization;

    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Protocols.Ximple;
    using Gorba.Common.Protocols.Ximple.Generic;
    using Gorba.Motion.Protran.AbuDhabi.Multiplexing;

    /// <summary>
    /// Base class for classes implementing the <see cref="IXimpleSource"/>.
    /// </summary>
    public abstract class XimpleSourceBase : IXimpleSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XimpleSourceBase"/> class.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        protected XimpleSourceBase(Dictionary dictionary)
        {
            this.Dictionary = dictionary;
        }

        /// <summary>
        /// Event that is fired every time a new Ximple object is created.
        /// This event is not fired when the ISI client is not supposed to send any
        /// Ximple (when in fallback mode).
        /// </summary>
        public event EventHandler<XimpleEventArgs> XimpleCreated;

        /// <summary>
        /// Event that is fired every for Ximple data that is to be sent immediately
        /// to Infomedia (without going through the arbiter and the cache).
        /// </summary>
        public event EventHandler<XimpleEventArgs> PriorityXimpleCreated;

        /// <summary>
        /// Gets the generic view dictionary.
        /// </summary>
        public Dictionary Dictionary { get; private set; }

        /// <summary>
        /// Raises the <see cref="PriorityXimpleCreated"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaisePriorityXimpleCreated(XimpleEventArgs e)
        {
            var handler = this.PriorityXimpleCreated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="XimpleCreated"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseXimpleCreated(XimpleEventArgs e)
        {
            var handler = this.XimpleCreated;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Searches the cell having the coordinate with a specific usage inside a XIMPLE.
        /// </summary>
        /// <param name="ximple">The XIMPLE in which perform the search.</param>
        /// <param name="usage">The usage having the coordinates to be searched in the XIMPLE.</param>
        /// <returns>The cell having the usage's coordinates, or null if the search fails.</returns>
        protected XimpleCell SearchForCellWithSpecificUsage(Ximple ximple, GenericUsage usage)
        {
            if (usage == null)
            {
                // not configured
                return null;
            }

            var table = this.Dictionary.GetTableForNameOrNumber(usage.Table);
            if (table == null)
            {
                // table not found.
                return null;
            }

            var column = table.GetColumnForNameOrNumber(usage.Column);
            if (column == null)
            {
                // column not found.
                return null;
            }

            var cell =
                ximple.Cells.Find(
                    c =>
                    c.TableNumber == table.Index
                    && c.ColumnNumber == column.Index
                    && c.RowNumber.ToString(CultureInfo.InvariantCulture) == usage.Row);
            return cell;
        }
    }
}