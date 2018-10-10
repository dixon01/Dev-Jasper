// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogFileUploadRequest.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LogFileUploadRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel
{
    using System.IO;
    using System.ServiceModel;

    using Gorba.Center.Common.ServiceModel.Units;

    /// <summary>
    /// The log file upload request.
    /// </summary>
    [MessageContract]
    public class LogFileUploadRequest
    {
        /// <summary>
        /// Gets or sets the unit id for which the log file is being uploaded.
        /// <seealso cref="Unit.Id"/>
        /// </summary>
        [MessageHeader(MustUnderstand = true)]
        public int UnitId { get; set; }

        /// <summary>
        /// Gets or sets the name (without path) of the log file.
        /// </summary>
        [MessageHeader(MustUnderstand = true)]
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets the content of the log file.
        /// </summary>
        [MessageBodyMember]
        public Stream Content { get; set; }
    }
}