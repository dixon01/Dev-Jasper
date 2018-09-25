// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetIqubeCmdMessageContext.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the QnetIqubeCmdMessageContext class. This class is used to make the protocol qnet
//   independant from Comms message
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System;

    /// <summary>
    ///  Defines the QnetIqubeCmdMessageContext class. This class is used to make the protocol qnet 
    /// independant from Comms message  
    /// </summary>
    public class QnetIqubeCmdMessageContext
    {
        /// <summary>
        /// Gets or sets TaskId.
        /// </summary>
        public uint TaskId { get; set; }

        /// <summary>
        /// Gets or sets the row id of the iqube where the text should be shown.
        /// </summary>
        public byte RowId { get; set; }

        /// <summary>
        /// Gets or sets Text to be displayed on iqube
        /// </summary>
        public string Text { get; set; }

        #region Fields for Info line text
        /// <summary>
        /// Gets or sets the font used to display the message.
        /// </summary>
        public sbyte Font { get; set; }

        /// <summary>
        /// Gets or sets the aligment of the text: Left, Center, Right
        /// </summary>
        public byte Align { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the text has to blink or not.
        /// </summary>
        public bool Blink { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether  the text has to scroll or not.
        /// </summary>
        public bool Scroll { get; set; }

        /// <summary>
        /// Gets or sets the Info line flags : See enum <see cref="InfoLineFlags"/> for available values.
        /// </summary>
        public sbyte Flags { get; set; }

        /// <summary>
        /// Gets or sets Res
        /// </summary>
        public sbyte ResAsBoolean { get; set; }

        /// <summary>
        /// Gets or sets Res 
        /// </summary>
        public ushort ResAsWord { get; set; }

        /// <summary>
        /// Gets or sets Side of the iqube display.
        /// </summary>
        public byte Side { get; set; }

        /// <summary>
        /// Gets or sets the start date of the activity.
        /// When <b>null</b>, the activity should start immediately.
        /// </summary>
        /// <value>
        /// The start date.
        /// </value>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the stop date of the activity.
        /// When <b>null</b>, the activity is valid until explicit revoke.
        /// </summary>
        /// <value>
        /// The stop date.
        /// </value>
        public DateTime? StopDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is scheduled daily.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is scheduled daily; otherwise, <c>false</c>.
        /// </value>
        public bool IsScheduledDaily { get; set; }
    
        #endregion
    }
}
