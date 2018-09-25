// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHttpChannel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IHttpChannel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Communication
{
    using System;
    using System.Net;

    /// <summary>
    /// Interface for an http channel.
    /// </summary>
    public interface IHttpChannel : IDisposable
    {
        /// <summary>
        /// Defines event handler to handle received messages
        /// </summary>
        event MessageReceivedEventHandler ResponseMessageReceived;

        /// <summary>
        /// Define event by receiving listener messages.
        /// </summary>
        event ListenerEventHandler ListenerMessageReceived;

        /// <summary>
        /// Occurs when a request of the HTTP channel failed.
        /// </summary>
        event EventHandler<ServiceUnavailableEventArgs> ServiceUnavailable;

        /// <summary>
        /// Gets HTTP response message queue
        /// </summary>
        HttpMessageQueue ResponseMessageQueue { get; }

        /// <summary>
        /// Gets HTTP listener message queue
        /// </summary>
        HttpMessageQueue ListenerMessageQueue { get; }

        /// <summary>
        /// Gets or sets HTTP server host address
        /// </summary>
        string HttpServerHost { get; set; }

        /// <summary>
        /// Gets or sets HTTP server port number
        /// </summary>
        string HttpServerPort { get; set; }

        /// <summary>
        /// Gets or sets HTTP listener host address
        /// </summary>
        string HttpListenerHost { get; set; }

        /// <summary>
        /// Gets or sets HTTP listener port number
        /// </summary>
        string HttpListenerPort { get; set; }

        /// <summary>
        /// Gets or sets HTTP configClient identification
        /// </summary>
        string HttpClientIndentification { get; set; }

        /// <summary>
        /// Gets or sets HTTP server identification
        /// </summary>
        string HttpServerIndentification { get; set; }

        /// <summary>
        /// Gets or sets HTTP server URL
        /// </summary>
        string HttpServerUrl { get; set; }

        /// <summary>
        /// Gets or sets HTTP listener URL
        /// </summary>
        string HttpListenerUrl { get; set; }

        /// <summary>
        /// Gets or sets HTTP web proxy host address
        /// </summary>
        string HttpWebProxyHost { get; set; }

        /// <summary>
        /// Gets or sets HTTP web proxy port number
        /// </summary>
        int HttpWebProxyPort { get; set; }

        /// <summary>
        /// Gets or sets HTTP response timeout
        /// </summary>
        int HttpResponseTimeout { get; set; }

        /// <summary>
        /// Gets the HTTP connection status
        /// </summary>
        HttpStatusCode HttpStatus { get; }

        /// <summary>
        /// Gets or sets the XML namespace.
        /// </summary>
        /// <value>
        /// The XML namespace.
        /// </value>
        string XmlNamespaceRequest { get; set; }

        /// <summary>
        /// Gets or sets the XML namespace response.
        /// </summary>
        /// <value>
        /// The XML namespace response.
        /// </value>
        string XmlNamespaceResponse { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to omit the xml declaration.
        /// </summary>
        bool OmitXmlDeclaration { get; set; }

        /// <summary>
        /// Start HTTP Listener.
        /// </summary>
        /// <param name="prefix">URI prefix to be handled by the HTTP listener</param>
        void StartListener(string prefix);

        /// <summary>
        /// Stop HTTP Listener.
        /// </summary>
        void StopListener();

        /// <summary>
        /// Set HTTP configuration parameters.
        /// </summary>
        /// <param name="vdvConfig">
        /// VDV configuration
        /// </param>
        void SetConfiguration(HttpChannelConfig vdvConfig);

        /// <summary>
        /// Send HTTP request message to the defined target for the defined service.
        /// </summary>
        /// <param name="target">
        /// Message target
        /// </param>
        /// <param name="service">
        /// Service name
        /// </param>
        /// <param name="message">
        /// Message text
        /// </param>
        /// <returns>
        /// indicates success of execution
        /// </returns>
        bool SendRequest(string target, string service, string message);

        /// <summary>
        /// Execute HTTP request message with the defined target for the defined service.
        /// The HTTP request message is sent and the response message will be returned to the caller.
        /// </summary>
        /// <param name="target">
        /// Message target
        /// </param>
        /// <param name="service">
        /// Service name
        /// </param>
        /// <param name="message">
        /// Message text
        /// </param>
        /// <returns>
        /// response message, may be null
        /// </returns>
        string ExecuteRequest(string target, string service, string message);

        /// <summary>
        /// Synchronized waiting for messages received by HTTP listener.
        /// </summary>
        void ReceiveListenerMessage();

        /// <summary>
        /// Sends HTTP listener response.
        /// </summary>
        /// <param name="message">
        /// message text
        /// </param>
        /// <param name="context">
        /// HTTP listener context
        /// </param>
        /// <param name="statusCode">
        /// The <see cref="HttpStatusCode"/>. The default value is HttpStatusCode.OK
        /// </param>
        /// <param name="statusDescription">
        /// The status description. The default value is "OK"
        /// </param>
        void SendListenerResponse(
            string message,
            HttpListenerContext context,
            HttpStatusCode statusCode = HttpStatusCode.OK,
            string statusDescription = "OK");
    }
}