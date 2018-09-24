// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetDisplayTypeInfolineRow.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    /// <summary>
    /// Contains all data representing the current displayed row on a Infoline type display.
    /// </summary>
    public class QnetDisplayTypeInfolineRow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QnetDisplayTypeInfolineRow"/> class.
        /// </summary>
        public QnetDisplayTypeInfolineRow()
        {
            this.Infoline = new QnetRealtimeDataValue();
        }

        /// <summary>
        /// Gets or sets the row number of the displayed text on iqube.  
        /// </summary>
        public int RowNumber { get; set; }

        /// <summary>
        /// Gets the current infoline displayed on the iqube.
        /// </summary>
        public QnetRealtimeDataValue Infoline { get; private set; }
    }
}
