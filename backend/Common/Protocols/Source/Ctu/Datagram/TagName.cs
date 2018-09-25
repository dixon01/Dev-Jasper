// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TagName.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ctu.Datagram
{
    /// <summary>
    /// All valid CTU tag numbers
    /// </summary>
    public enum TagName
    {
        // Notifications

        /// <summary>
        /// Status = 1
        /// </summary>
        Status = 1,

        /// <summary>
        /// LogMessage = 2
        /// </summary>
        LogMessage = 2,

        /// <summary>
        /// DisplayStatus = 3
        /// </summary>
        DisplayStatus = 3,

        /// <summary>
        /// TripInfo = 100
        /// </summary>
        TripInfo = 100,

        /// <summary>
        /// DownloadStart = 101
        /// </summary>
        DownloadStart = 101,

        /// <summary>
        /// DownloadAbort = 102
        /// </summary>
        DownloadAbort = 102,

        /// <summary>
        /// LineInfo = 103
        /// </summary>
        LineInfo = 103,

        /// <summary>
        /// LineInfo = 104
        /// </summary>
        ExtendedLineInfo = 104,

        /// <summary>
        /// CountdownNumber = 105
        /// </summary>
        CountdownNumber = 105,

        /// <summary>
        /// SpecialInputInfo = 106
        /// </summary>
        SpecialInputInfo = 106,

        /// <summary>
        /// ExteriorSignTexts = 120
        /// </summary>
        ExteriorSignTexts = 120,

        // Requests

        /// <summary>
        /// DeviceInfoRequest = 1025
        /// </summary>
        DeviceInfoRequest = 1025,

        /// <summary>
        /// DownloadProgressRequest = 1026
        /// </summary>
        DownloadProgressRequest = 1026,

        // Responses

        /// <summary>
        /// DeviceInfoResponse = 2049
        /// </summary>
        DeviceInfoResponse = 2049,

        /// <summary>
        /// DownloadProgressResponse = 2050
        /// </summary>
        DownloadProgressResponse = 2050
    }
}