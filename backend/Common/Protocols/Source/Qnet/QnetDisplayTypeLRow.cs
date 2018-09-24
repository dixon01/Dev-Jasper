// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetDisplayTypeLRow.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the QnetDisplayTypeC type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    /// <summary>
    /// Contains all data representing the current displayed row on a S type display.
    /// </summary>
    public class QnetDisplayTypeLRow
    {
        /// <summary>
        /// Number max of rows for display type L
        /// </summary>
        public const int RowCount = 3;

        /// <summary>
        /// Initializes a new instance of the <see cref="QnetDisplayTypeLRow"/> class.
        /// </summary>
        public QnetDisplayTypeLRow()
        {
            this.Line = new QnetRealtimeDataValue();
            this.Destination = new QnetRealtimeDataValue();
            this.DepartureTime = new QnetRealtimeDataValue();
            this.Lane = new QnetRealtimeDataValue();
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

        /// <summary>
        /// Gets the current lane displayed on the iqube. 
        /// </summary>
        public QnetRealtimeDataValue Lane { get; private set; }    
    }
}
