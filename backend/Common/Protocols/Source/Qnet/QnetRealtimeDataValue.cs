// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetRealtimeDataValue.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Information of a displayed text in the realtime data row.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    /// <summary>
    /// Information of a displayed text in the realtime data row.
    /// </summary>
    public class QnetRealtimeDataValue
    {
        /// <summary>
        /// Gets or sets Alignment of the text value.
        /// </summary>
        public int Alignment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the text is blinking.
        /// </summary>
        public bool IsBlinking { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the text is scrolling.
        /// </summary>
        public bool IsScrolling { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the row is valid.
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether AutoInfoLine.
        /// </summary>
        public bool AutoInfoLine { get; set; }

        /// <summary>
        /// Gets or sets text of the cell.
        /// </summary>
        public string Text { get; set; }
    }
}
