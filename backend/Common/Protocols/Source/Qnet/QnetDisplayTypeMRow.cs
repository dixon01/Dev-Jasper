// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetDisplayTypeMRow.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the QnetDisplayTypeM type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    /// <summary>
    /// Contains all data representing the current displayed row on a M type display.
    /// </summary>
    public class QnetDisplayTypeMRow
    {
        /// <summary>
        /// Number max of rows for display type M
        /// </summary>
        public const int RowCount = 4;

        /// <summary>
        /// Initializes a new instance of the <see cref="QnetDisplayTypeMRow"/> class.
        /// </summary>
        public QnetDisplayTypeMRow()
        {
            this.Line = new QnetRealtimeDataValue();
            this.Destination = new QnetRealtimeDataValue();
            this.DepartureTime = new QnetRealtimeDataValue();
        }

        /// <summary>
        /// Gets or sets the row number of the displayed text on iqube.  
        /// </summary>
        public int RowNumber { get; set; }

        /// <summary>
        /// Gets the current line displayed on the iqube. 
        /// </summary>
        public QnetRealtimeDataValue Line { get; private set; }

        /// <summary>
        /// Gets the current destination displayed on the iqube. 
        /// </summary>
        public QnetRealtimeDataValue Destination { get; private set; }

        /// <summary>
        /// Gets the current departure time displayed on the iqube. 
        /// </summary>
        public QnetRealtimeDataValue DepartureTime { get; private set; }
    }
}
