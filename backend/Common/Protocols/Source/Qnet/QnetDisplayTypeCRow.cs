// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetDisplayTypeCRow.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Contains all data representing the current displayed row on a C type display.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    /// <summary>
    /// Contains all data representing the current displayed row on a C type display.
    /// </summary>
    public class QnetDisplayTypeCRow
    {
        /// <summary>
        /// Number max of rows
        /// </summary>
        public const int RowCount = 8;

        /// <summary>
        /// Gets or sets the character displayed at the left digit
        /// </summary>
        public char TensChar { get; set; } 

        /// <summary>
        /// Gets or sets the character displayed at the right digit
        /// </summary>
        public char UnitsChar { get; set; } 

        /// <summary>
        /// Gets or sets a value indicating whether the Blink.
        /// </summary>
        public bool Blink { get; set; } 
        }
}
