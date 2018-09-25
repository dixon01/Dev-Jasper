// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RecordingFormat.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RecordingFormat type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis
{
    /// <summary>
    /// The format in which to record telegrams.
    /// </summary>
    public enum RecordingFormat
    {
        /// <summary>
        /// The proprietary Protran format.
        /// </summary>
        Protran,

        /// <summary>
        /// The Gismo format - currently not supported.
        /// </summary>
        Gismo
    }
}