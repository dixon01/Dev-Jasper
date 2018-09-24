// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiProtocolTransceiverDescriptor.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MultiProtocolTransceiverDescriptor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareDescription
{
    /// <summary>
    /// The multi-protocol transceiver descriptor.
    /// </summary>
    public class MultiProtocolTransceiverDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiProtocolTransceiverDescriptor"/> class.
        /// </summary>
        public MultiProtocolTransceiverDescriptor()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiProtocolTransceiverDescriptor"/> class.
        /// </summary>
        /// <param name="index">
        /// The <see cref="Index"/>.
        /// </param>
        public MultiProtocolTransceiverDescriptor(int index)
        {
            this.Index = index;
        }

        /// <summary>
        /// Gets or sets the index of the transceiver used by Hardware Manager.
        /// This index is from 0 (contrary to HMW configuration where it is from 1).
        /// </summary>
        public int Index { get; set; }
    }
}