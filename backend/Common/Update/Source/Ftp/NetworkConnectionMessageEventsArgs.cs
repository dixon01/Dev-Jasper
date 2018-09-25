namespace Gorba.Common.Update.Ftp
{
    using System;

    /// <summary>The network connection message events args.</summary>
    public class NetworkConnectionMessageEventsArgs : EventArgs
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="NetworkConnectionMessageEventsArgs"/> class.</summary>
        /// <param name="connected">The connected.</param>
        public NetworkConnectionMessageEventsArgs(bool connected)
        {
            this.Connected = connected;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets a value indicating whether connected.</summary>
        public bool Connected { get; set; }

        #endregion
    }
}