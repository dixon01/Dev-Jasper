// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetVdvMessageContext.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the QnetVdvMessageContext class. This class is used to make the protocol qnet
//   independant from Comms message
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System;

    using Gorba.Common.Protocols.Qnet.Structures;

    /// <summary>
    ///  Defines the QnetVdvMessageContext class. This class is used to make the protocol qnet 
    /// independant from Comms message  
    /// </summary>
    public class QnetVdvMessageContext
    {
        public ushort ITCSIdentifier { get; set; }
        public uint LineId { get; set; }
        public ushort DestinationId { get; set; }
        public uint TripId { get; set; }
        public ushort StopSequenceCounter { get; set; }

        public uint ScheduledArrivalTime { get; set; }
        public uint ScheduledDepartureTime { get; set; }
        public uint LineTextId { get; set; }
        public uint Destination1TextId { get; set; }
        public uint Destination2TextId { get; set; }
        public uint PlatformTextId { get; set; }
 
        public uint EstimatedArrivalTime { get; set; }
        public uint EstimatedDepartureTime { get; set; }
        public bool ContainsRealetime { get; set; }
        public uint AnAbmeldeId { get; set; }
        public bool IsAtStation { get; set; }

        /// <summary>
        /// Gets or sets VdvTrafficJamIndicator. <see cref="VdvTrafficType"/> for more details.
        /// </summary>
        public VdvTrafficType VdvTrafficJamIndicator { get; set; }

        /// <summary>
        /// Gets or sets the reference text identifier. The reference text identifier is used to 
        /// reference the strings into vdv messages.
        /// </summary>
        public uint ReferenceTextId { get; set; }

        /// <summary>
        /// Gets or sets the font number. It's the font thjat will be used to display the message.
        /// </summary>
        public byte FontNumber { get; set; }

        /// <summary>
        /// Gets or sets the type of the reference text. See <see cref="VdvReferenceTextType"/> for the list of accepted types.
        /// </summary>
        public VdvReferenceTextType ReferenceType { get; set; }

        /// <summary>
        /// Gets or sets the text to be displayed
        /// </summary>
        public string DisplayText { get; set; }

        /// <summary>
        /// Gets or sets the text for Text To Speech
        /// </summary>
        public string TTSText { get; set; }

        /// <summary>
        /// Gets or sets the line number
        /// </summary>
        public uint LineNumber { get; set; }

        /// <summary>
        /// Gets or sets the experition time of the info line 
        /// </summary>
        public DateTime InfoLineExpirationTime { get; set; }
    }
}
