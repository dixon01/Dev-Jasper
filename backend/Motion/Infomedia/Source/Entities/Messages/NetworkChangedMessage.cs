// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="NetworkChangedMessage.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Motion.Infomedia.Entities.Messages
{
    using System;

    using Gorba.Common.Medi.Core.Messages;

    /// <summary>The network connection message.</summary>
    [Serializable]
    public class NetworkChangedMessage
    {
        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="NetworkChangedMessage" /> class.</summary>
        public NetworkChangedMessage()
            : this(false)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="NetworkChangedMessage"/> class.</summary>
        /// <param name="wifiConnected">The wifi connected flag set true to indicate a network connection exists.</param>
        public NetworkChangedMessage(bool wifiConnected)
        {
            this.WiFiConnected = wifiConnected;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the host.</summary>
        public string Host { get; set; }

        /// <summary>Gets or sets the password.</summary>
        public string Password { get; set; }

        /// <summary>Gets or sets the user name.</summary>
        public string Username { get; set; }

        /// <summary>Gets or sets a value indicating whether the wifi network is connected.</summary>
        public bool WiFiConnected { get; set; }

        #endregion
    }
}