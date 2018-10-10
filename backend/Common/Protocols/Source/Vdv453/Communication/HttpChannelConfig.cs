// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpChannelConfig.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HttpChannelConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Communication
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Configuration for an <see cref="HttpChannel"/>.
    /// </summary>
    public class HttpChannelConfig
    {
        /// <summary>
        /// Gets or sets the remote server host name.
        /// </summary>
        public string ServerHost { get; set; }

        /// <summary>
        /// Gets or sets the remote server port.
        /// </summary>
        public decimal? ServerPort { get; set; }

        /// <summary>
        /// Gets or sets the server identification.
        /// </summary>
        public string ServerIdentification { get; set; }

        /// <summary>
        /// Gets or sets the local listener host name.
        /// </summary>
        public string ListenerHost { get; set; }

        /// <summary>
        /// Gets or sets the local listener port.
        /// </summary>
        public decimal? ListenerPort { get; set; }

        /// <summary>
        /// Gets or sets the client identification.
        /// </summary>
        public string ClientIdentification { get; set; }

        /// <summary>
        /// Gets or sets the host name of the web proxy to use 
        /// for outgoing connections.
        /// </summary>
        public string WebProxyHost { get; set; }

        /// <summary>
        /// Gets or sets the port of the web proxy to use 
        /// for outgoing connections.
        /// </summary>
        public decimal? WebProxyPort { get; set; }

        /// <summary>
        /// Gets or sets the response timeout.
        /// </summary>
        public TimeSpan ResponseTimeOut { get; set; }

        /// <summary>
        /// Gets or sets the wrong simulated subscriptions count.
        /// </summary>
        public int WrongSimulatedSubscriptionsCount { get; set; }

        /// <summary>
        /// Gets or sets the XML namespace used for messages sent to the client.
        /// </summary>
        /// <value>
        /// The XML namespace.
        /// </value>
        [DisplayName("Xmlns response")]
        public string XmlNamespaceResponse { get; set; }

        /// <summary>
        /// Gets or sets the XML namespace used in requests by the client.
        /// </summary>
        /// <value>
        /// The XML namespace request.
        /// </value>
        [DisplayName("Xmlns request")]
        public string XmlNamespaceRequest { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the xml declaration should be omitted.
        /// </summary>
        public bool OmitXmlDeclaration { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("HttpChannel configuration");
        }
    }
}