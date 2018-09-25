// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileTransportServerConfig.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileTransportServerConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.File
{
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// Configuration for the <see cref="FileTransportServer"/>.
    /// </summary>
    [Implementation(typeof(FileTransportServer))]
    public class FileTransportServerConfig : TransportServerConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileTransportServerConfig"/> class.
        /// </summary>
        public FileTransportServerConfig()
        {
            // TODO: set better default values
            this.DropLocation = "file:///C:/temp/medi_drop";
            this.Username = string.Empty;
            this.Password = string.Empty;

            this.PollInterval = 4000;
            this.MessageTimeToLive = this.PollInterval * 4;
        }

        /// <summary>
        /// Gets or sets the URL of the location to drop the files.
        /// Currently only the "file://" schema is supported.
        /// </summary>
        public string DropLocation { get; set; }

        /// <summary>
        /// Gets or sets the user name used to authenticate at the drop location.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password used to authenticate at the drop location.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the time to live of messages in seconds.
        /// Messages sent will have a timestamp with this
        /// TTL added. After the expiry of the TTL, the
        /// message will be discarded by peers or deleted
        /// by the sender or any other peer.
        /// </summary>
        public int MessageTimeToLive { get; set; }

        /// <summary>
        /// Gets or sets the polling interval in milliseconds.
        /// Every so often the the protocol will check if new
        /// files are available.
        /// </summary>
        public int PollInterval { get; set; }
    }
}
