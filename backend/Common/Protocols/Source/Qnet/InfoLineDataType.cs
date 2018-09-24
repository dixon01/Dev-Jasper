// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InfoLineDataType.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Enumerates the list of available DataType that can be defined to send an Info line to an iqube.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    /// <summary>
    /// Enumerates the list of available DataType that can be defined to send an Info line to an iqube. 
    /// </summary>
    public enum InfoLineDataType
    {
        /// <summary>
        /// Data type is not set
        /// </summary>
        None = -1,

        /// <summary>
        /// Data type is data, that means text to display
        /// </summary>
        Data,

        /// <summary>
        /// Bipmap to draw
        /// </summary>
        Bipmap,

        /// <summary>
        /// Seq file to read to display text
        /// </summary>
        SeqFile,
    }
}
