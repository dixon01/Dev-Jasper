// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventHandlerPeerConfigBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EventHandlerPeerConfigBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Config
{
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// The base class for EventHandler client and server configuration.
    /// </summary>
    public abstract class EventHandlerPeerConfigBase : PeerConfig
    {
        /// <summary>
        /// The default port (1598).
        /// </summary>
        protected const int DefaultPort = 1598;

        private const string DefaultEncoding = "utf-8";

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerPeerConfigBase"/> class.
        /// </summary>
        protected EventHandlerPeerConfigBase()
        {
            this.SupportedMessages = new List<string>();
            this.Encoding = DefaultEncoding;
        }

        /// <summary>
        /// Gets or sets the list of full class names of the messages
        /// supported by the EventHandler peer.
        /// </summary>
        [Editor(
            "System.Windows.Forms.Design.ListControlStringCollectionEditor, " +
            "System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Drawing.Design.UITypeEditor, " +
            "System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public List<string> SupportedMessages { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this peer should run in tagged mode.
        /// Tagged mode means that every XML object is embedded in &lt;|1&gt;...&lt;|2&gt;.
        /// Tagged mode is used on VM.x with EdiServer since it requires those tags.
        /// </summary>
        [DefaultValue(false)]
        public bool TaggedMode { get; set; }

        /// <summary>
        /// Gets or sets the encoding to be used.
        /// Default is utf-8.
        /// </summary>
        [DefaultValue(DefaultEncoding)]
        public string Encoding { get; set; }
    }
}