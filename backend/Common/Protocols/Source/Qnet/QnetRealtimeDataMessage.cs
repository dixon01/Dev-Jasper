// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetRealtimeDataMessage.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Qnet message request for time synchronization with a server
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System.Collections.Generic;

    /// <summary>
    /// Qnet message request for time synchronization with a server
    /// </summary>
    public class QnetRealtimeDataMessage : QnetMessageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QnetRealtimeDataMessage"/> class.
        /// </summary>
        /// <param name="sourceAdress">
        /// The source address.
        /// </param>
        /// <param name="destAddr">
        /// The destination address.
        /// </param>
        /// <param name="gatewayAddress">
        /// The gateway Address.
        /// </param>
        public QnetRealtimeDataMessage(ushort sourceAdress, ushort destAddr, ushort gatewayAddress)
            : base(sourceAdress, destAddr, gatewayAddress)
        {
            this.DisplayTypeCRows = new List<QnetDisplayTypeCRow>();
            for (int i = 0; i < QnetDisplayTypeCRow.RowCount; i++)
            {
                this.DisplayTypeCRows.Add(new QnetDisplayTypeCRow());
            }

            this.DisplayTypeLRows = new List<QnetDisplayTypeLRow>();
            for (int i = 0; i < QnetDisplayTypeLRow.RowCount; i++)
            {
                this.DisplayTypeLRows.Add(new QnetDisplayTypeLRow());
            }

            this.DisplayTypeMRows = new List<QnetDisplayTypeMRow>();
            for (int i = 0; i < QnetDisplayTypeMRow.RowCount; i++)
            {
                this.DisplayTypeMRows.Add(new QnetDisplayTypeMRow());
            }

            this.DisplayTypeSRow = new QnetDisplayTypeSRow();

            this.DisplayTypeInfolineRow = new QnetDisplayTypeInfolineRow();
        }

        /// <summary>
        /// Gets or sets the DisplayType. See <see cref="RealtimeDisplayType"/> for more details.
        /// </summary>
        public RealtimeDisplayType DisplayType { get; set; }

        /// <summary>
        /// Gets the collection of all <see cref="QnetDisplayTypeCRow"/>.
        /// </summary>
        public List<QnetDisplayTypeCRow> DisplayTypeCRows { get; private set; }

        /// <summary>
        /// Gets the DisplayTypeSRow.
        /// </summary>
        public QnetDisplayTypeSRow DisplayTypeSRow { get; private set; }

        /// <summary>
        /// Gets the collection of all <see cref="QnetDisplayTypeLRow"/>.
        /// </summary>
        public List<QnetDisplayTypeLRow> DisplayTypeLRows { get; private set; }

        /// <summary>
        /// Gets the collection of all <see cref="QnetDisplayTypeMRow"/>.
        /// </summary>
        public List<QnetDisplayTypeMRow> DisplayTypeMRows { get; private set; }

        /// <summary>
        /// Gets the <see cref="QnetDisplayTypeInfolineRow"/>.
        /// </summary>
        public QnetDisplayTypeInfolineRow DisplayTypeInfolineRow { get; private set; }
    }
}