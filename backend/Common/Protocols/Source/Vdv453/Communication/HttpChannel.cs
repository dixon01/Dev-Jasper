// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpChannel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implements a class that implements the HTTP communication Layer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Communication
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;

    using NLog;

    /// <summary>
    /// Defines handler for the HTTP Web request events.
    /// </summary>
    /// <param name="o">
    /// Message received
    /// </param>
    public delegate void MessageReceivedEventHandler(object o);

    /// <summary>
    /// Defines handler for the HTTP listener events.
    /// </summary>
    /// <param name="o">
    /// Message received
    /// </param>
    public delegate void ListenerEventHandler(object o);

    /// <summary>
    /// Class HttpChannel is the concrete realization of the HTTP communication Layer.
    /// It sends messages to the HTTP server and uses HTTPListener to receive messages from the server.
    /// Received messages will be forwarded to higher levels by events.
    /// </summary>
    public sealed class HttpChannel : IHttpChannel
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// lock object for HTTP listener
        /// </summary>
        private readonly object httpListenerLock;

        /// <summary>
        /// lock object for HTTP client
        /// </summary>
        private readonly object httpClientLock;

        private bool serviceUnavailableEventRaised;

        /// <summary>
        /// HTTP server host address
        /// </summary>
        private string httpServerHost = string.Empty;

        /// <summary>
        /// HTTP Server port number
        /// </summary>
        private string httpServerPort = string.Empty;

        /// <summary>
        /// HTTP listener host address
        /// </summary>
        private string httpListenerHost = string.Empty;

        /// <summary>
        /// HTTP listener port number
        /// </summary>
        private string httpListenerPort = string.Empty;

        /// <summary>
        /// HTTP client identification
        /// </summary>
        private string httpClientIndentification = string.Empty;

        /// <summary>
        /// HTTP server identification
        /// </summary>
        private string httpServerIndentification = string.Empty;

        /// <summary>
        /// HTTP web proxy host address
        /// </summary>
        private string httpWebProxyHost = string.Empty;

        /// <summary>
        /// HTTP web proxy host port number
        /// </summary>
        private int httpWebProxyPort;

        /// <summary>
        /// HTTP response timeout
        /// </summary>
        private int httpResponseTimeout = 30000; // default value [ms]

        /// <summary>
        /// HTTP server URL
        /// </summary>
        private string httpServerUrl = string.Empty;

        /// <summary>
        /// HTTP listener URL
        /// </summary>
        private string httpListenerUrl = string.Empty;

        /// <summary>
        /// HTTP web request instance
        /// </summary>
        private HttpWebRequest httpRequest;

        /// <summary>
        /// HTTP listener instance
        /// </summary>
        private HttpListener httpListener;

        /// <summary>
        /// HTTP status
        /// </summary>
        private HttpStatusCode httpStatus = HttpStatusCode.Continue;

        /// <summary>
        /// Initializes a new instance of the HttpChannel class.
        /// </summary>
        public HttpChannel()
        {
            this.httpListenerLock = new object();
            this.httpClientLock = new object();

            this.ResponseMessageQueue = new HttpMessageQueue();
            this.ListenerMessageQueue = new HttpMessageQueue();

            Logger.Trace("HttpChannel instance created.");
        }

        /// <summary>
        /// Defines event handler to handle received messages
        /// </summary>
        public event MessageReceivedEventHandler ResponseMessageReceived;

        /// <summary>
        /// Define event by receiving listener messages.
        /// </summary>
        public event ListenerEventHandler ListenerMessageReceived;

        /// <summary>
        /// Occurs when a request of the HTTP channel failed.
        /// </summary>
        public event EventHandler<ServiceUnavailableEventArgs> ServiceUnavailable;

        /// <summary>
        /// Gets HTTP response message queue
        /// </summary>
        public HttpMessageQueue ResponseMessageQueue { get; private set; }

        /// <summary>
        /// Gets HTTP listener message queue
        /// </summary>
        public HttpMessageQueue ListenerMessageQueue { get; private set; }

        /// <summary>
        /// Gets or sets HTTP server host address
        /// </summary>
        public string HttpServerHost
        {
            get
            {
                return this.httpServerHost;
            }

            set
            {
                this.httpServerHost = value;
            }
        }

        /// <summary>
        /// Gets or sets HTTP server port number
        /// </summary>
        public string HttpServerPort
        {
            get
            {
                return this.httpServerPort;
            }

            set
            {
                this.httpServerPort = value;
            }
        }

        /// <summary>
        /// Gets or sets HTTP listener host address
        /// </summary>
        public string HttpListenerHost
        {
            get
            {
                return this.httpListenerHost;
            }

            set
            {
                this.httpListenerHost = value;
            }
        }

        /// <summary>
        /// Gets or sets HTTP listener port number
        /// </summary>
        public string HttpListenerPort
        {
            get
            {
                return this.httpListenerPort;
            }

            set
            {
                this.httpListenerPort = value;
            }
        }

        /// <summary>
        /// Gets or sets HTTP configClient identification
        /// </summary>
        public string HttpClientIndentification
        {
            get
            {
                return this.httpClientIndentification;
            }

            set
            {
                this.httpClientIndentification = value;
            }
        }

        /// <summary>
        /// Gets or sets HTTP server identification
        /// </summary>
        public string HttpServerIndentification
        {
            get
            {
                return this.httpServerIndentification;
            }

            set
            {
                this.httpServerIndentification = value;
            }
        }

        /// <summary>
        /// Gets or sets HTTP server URL
        /// </summary>
        public string HttpServerUrl
        {
            get
            {
                return this.httpServerUrl;
            }

            set
            {
                this.httpServerUrl = value;
            }
        }

        /// <summary>
        /// Gets or sets HTTP listener URL
        /// </summary>
        public string HttpListenerUrl
        {
            get
            {
                return this.httpListenerUrl;
            }

            set
            {
                this.httpListenerUrl = value;
            }
        }

        /// <summary>
        /// Gets or sets HTTP web proxy host address
        /// </summary>
        public string HttpWebProxyHost
        {
            get
            {
                return this.httpWebProxyHost;
            }

            set
            {
                this.httpWebProxyHost = value;
            }
        }

        /// <summary>
        /// Gets or sets HTTP web proxy port number
        /// </summary>
        public int HttpWebProxyPort
        {
            get
            {
                return this.httpWebProxyPort;
            }

            set
            {
                this.httpWebProxyPort = value;
            }
        }

        /// <summary>
        /// Gets or sets HTTP response timeout
        /// </summary>
        public int HttpResponseTimeout
        {
            get
            {
                return this.httpResponseTimeout;
            }

            set
            {
                this.httpResponseTimeout = value;
            }
        }

        /// <summary>
        /// Gets the HTTP connection status
        /// </summary>
        public HttpStatusCode HttpStatus
        {
            get
            {
                return this.httpStatus;
            }
        }

        /// <summary>
        /// Gets or sets the XML namespace.
        /// </summary>
        /// <value>
        /// The XML namespace.
        /// </value>
        public string XmlNamespaceRequest { get; set; }

        /// <summary>
        /// Gets or sets the XML namespace response.
        /// </summary>
        /// <value>
        /// The XML namespace response.
        /// </value>
        public string XmlNamespaceResponse { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to omit the xml declaration.
        /// </summary>
        public bool OmitXmlDeclaration { get; set; }

        /// <summary>
        /// Start HTTP Listener.
        /// </summary>
        /// <param name="prefix">URI prefix to be handled by the HTTP listener</param>
        public void StartListener(string prefix)
        {
            try
            {
                if (!HttpListener.IsSupported)
                {
                    Logger.Error("Windows XP SP2, Server 2003 or later is mandatory for using HttpListener!");
                    return;
                }

                if (this.httpListener == null)
                {
                    this.httpListener = new HttpListener();

                    Logger.Debug("HttpListener created.");
                }

                this.httpListener.Prefixes.Clear();

                this.httpListener.Prefixes.Add(this.httpListenerUrl + prefix);

                Logger.Debug("Prefix {0} added.", prefix);

                this.httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
                this.httpListener.Start();
            }
            catch (Exception e)
            {
                Logger.ErrorException("StartListener Exception", e);
            }
        }

        /// <summary>
        /// Stop HTTP Listener.
        /// </summary>
        public void StopListener()
        {
            try
            {
                if (this.httpListener != null)
                {
                    this.httpListener.Stop();
                    this.httpListener.Prefixes.Clear();
                    this.httpListener.Abort();
                    this.httpListener.Close();
                    this.httpListener = null;
                }

                Logger.Debug("HttpListener stopped.");
            }
            catch (Exception e)
            {
                Logger.ErrorException("StopListener Exception", e);
            }
        }

        /// <summary>
        /// Set HTTP configuration parameters.
        /// </summary>
        /// <param name="vdvConfig">
        /// VDV configuration
        /// </param>
        public void SetConfiguration(HttpChannelConfig vdvConfig)
        {
            this.httpServerHost = vdvConfig.ServerHost;

            Logger.Debug("ServerHost: {0}", this.httpServerHost);

            this.httpServerPort = vdvConfig.ServerPort.ToString();

            Logger.Debug("ServerPort: {0}", this.httpServerPort);

            this.httpServerIndentification = vdvConfig.ServerIdentification;

            Logger.Debug("ServerIdentification: {0}", this.httpServerIndentification);

            this.httpListenerHost = vdvConfig.ListenerHost;

            Logger.Debug("ListenerHost: {0}", this.httpListenerHost);

            this.httpListenerPort = vdvConfig.ListenerPort.ToString();

            Logger.Debug("ListenerPort: {0}", this.httpListenerPort);

            this.httpClientIndentification = vdvConfig.ClientIdentification;

            Logger.Debug("ClientIdentification: {0}", this.httpClientIndentification);

            this.XmlNamespaceResponse = vdvConfig.XmlNamespaceResponse;
            Logger.Debug("XmlNamespaceResponse: '{0}'", this.XmlNamespaceResponse);

            this.XmlNamespaceRequest = vdvConfig.XmlNamespaceRequest;
            Logger.Debug("XmlNamespaceRequest: '{0}'", this.XmlNamespaceRequest);

            this.OmitXmlDeclaration = vdvConfig.OmitXmlDeclaration;
            Logger.Debug("OmitXmlDeclaration: {0}", this.OmitXmlDeclaration);

            // Web Proxy is optional!
            this.httpWebProxyHost = vdvConfig.WebProxyHost;

            Logger.Debug("WebProxyHost: {0}", this.httpWebProxyHost);

            if (vdvConfig.WebProxyPort.HasValue)
            {
                this.httpWebProxyPort = (int)vdvConfig.WebProxyPort.Value;
            }

            Logger.Debug("WebProxyPort: {0}", this.httpWebProxyPort);

            this.httpResponseTimeout = (int)vdvConfig.ResponseTimeOut.TotalSeconds * 1000;
            if (this.httpResponseTimeout > 0)
            {
                Logger.Debug("HttpResponseTimeout: {0}", this.httpResponseTimeout);
            }
            else
            {
                this.httpResponseTimeout = 30000;

                Logger.Debug("HttpResponseTimeout set to default value: {0}", this.httpResponseTimeout);
            }

            this.httpServerUrl = string.Format(
                "{0}:{1}/{2}/", this.httpServerHost, this.httpServerPort, this.httpServerIndentification);

            Logger.Debug("Server Connection URL: {0}", this.httpServerUrl);

            this.httpListenerUrl = string.Format(
                "{0}:{1}/{2}/", this.httpListenerHost, this.httpListenerPort, this.httpClientIndentification);

            Logger.Debug("Client Listener URL: {0}", this.httpListenerUrl);
        }

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
        public bool SendRequest(string target, string service, string message)
        {
            var result = this.SendRequestMessage(target, service, message);
            if (result)
            {
                // Get HTTP response
                return this.GetResponse();
            }

            Logger.Warn("Couldn't send the request message");
            return false;
        }

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
        public string ExecuteRequest(string target, string service, string message)
        {
            if (this.SendRequestMessage(target, service, message))
            {
                // Get HTTP response message
                var httpChannelResponse = this.GetResponseMessage();
                if (httpChannelResponse != null)
                {
                    return httpChannelResponse.Content;
                }

                Logger.Warn("Invalid or empty response");
                return null;
            }

            Logger.Warn("Couldn't send the request message");
            return null;
        }

        /// <summary>
        /// Synchronized waiting for messages received by HTTP listener.
        /// </summary>
        public void ReceiveListenerMessage()
        {
            try
            {
                Logger.Debug("HttpListener starts listening...");

                var result = this.httpListener.BeginGetContext(this.ListenerCallback, this.httpListener);

                result.AsyncWaitHandle.WaitOne();
            }
            catch (Exception e)
            {
                Logger.ErrorException("ReceiveListenerMessage Exception", e);
            }
        }

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
        /// The status Code. Default value is "OK"
        /// </param>
        /// <param name="statusDescription">
        /// The status Description. Default value is "OK"
        /// </param>
        public void SendListenerResponse(
              string message,
            HttpListenerContext context,
            HttpStatusCode statusCode = HttpStatusCode.OK,
            string statusDescription = "OK")
        {
            lock (this.httpListenerLock)
            {
                try
                {
                    ////Logger.Trace("In lock before sending HTTP response message.");

                    var encoding = Encoding.GetEncoding("iso-8859-1");

                    // Get HTTP listener response object
                    using (var response = context.Response)
                    {
                        // Set response content length
                        response.ContentLength64 = encoding.GetByteCount(message);

                        // Set the response http status code
                        response.StatusCode = (int)statusCode;
                        response.StatusDescription = statusDescription;

                        // Get response output stream
                        using (var writer = new StreamWriter(response.OutputStream, Encoding.GetEncoding("iso-8859-1")))
                        {
                            // Send response
                            writer.Write(message);
                        }
                    }

                    Logger.Debug("HttpListener response message sent.");
                }
                catch (Exception e)
                {
                    Logger.ErrorException("SendListenerResponse Exception", e);
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.StopListener();
        }

        private void RaiseServiceUnavailable(string address)
        {
            if (this.serviceUnavailableEventRaised)
            {
                Logger.Debug("ServiceUnavailable event already raised");
                return;
            }

            var handler = this.ServiceUnavailable;
            if (handler == null)
            {
                return;
            }

            this.serviceUnavailableEventRaised = true;
            var serverUnavailableEventARgs = new ServiceUnavailableEventArgs(address);
            handler(this, serverUnavailableEventARgs);
        }

        /// <summary>
        /// Listener Callback function will be called when the listener receives a message.
        /// </summary>
        /// <param name="result">
        /// status of asynchronous operation
        /// </param>
        private void ListenerCallback(IAsyncResult result)
        {
            lock (this.httpListenerLock)
            {
                Logger.Trace("ListenerCallback function started.");

                // Get Reference to HTTP listener, --> do not close!!!!!!!!!!!
                HttpListener listener = null;
                try
                {
                    listener = (HttpListener)result.AsyncState;
                }
                catch (Exception e)
                {
                    Logger.ErrorException("Listener Exception", e);
                }

                if (listener == null || !listener.IsListening)
                {
                    return;
                }

                try
                {
                    // Call EndGetContext to complete the asynchronous operation
                    var context = listener.EndGetContext(result);

                    // check if it is one of the listened addresses
                    var dataReadyRequest = context.Request;

                    Logger.Debug("Listener has received message from URI: {0}", dataReadyRequest.Url.AbsolutePath);

                    this.GetHttpListenerData(dataReadyRequest);

                    // Raise event - listener has received message
                    this.OnListenerMessageReceived(context);
                }
                catch (Exception e)
                {
                    Logger.ErrorException("ListenerCallback Exception", e);
                }
            }
        }

        /// <summary>
        /// Reads HTTP Listener data.
        /// </summary>
        /// <param name="request">
        /// incoming HTTP request
        /// </param>
        private void GetHttpListenerData(HttpListenerRequest request)
        {
            Logger.Trace("GetHttpListenerData started.");
            try
            {
                var buf = new byte[8192];
                var dataMsg = new StringBuilder();

                using (var resStream = request.InputStream)
                {
                    int count;
                    do
                    {
                        // Read data
                        count = resStream.Read(buf, 0, buf.Length);
                        if (count > 0)
                        {
                            // Encoding ist "iso-8859-1"
                            var tempString = Encoding.GetEncoding("iso-8859-1").GetString(buf, 0, count);

                            // continue building the string
                            dataMsg.Append(tempString);
                        }
                    }
                    while (count > 0);
                }

                Logger.Debug("HttpListener message received");
                Logger.Trace(dataMsg);

                var message = dataMsg.ToString();

                if (!string.IsNullOrEmpty(message))
                {
                    this.ListenerMessageQueue.Enqueue(message);
                }
            }
            catch (Exception e)
            {
                Logger.ErrorException("Read HttpListener message failed", e);
            }
        }

        /// <summary>
        /// Send HTTP request message to the defined target for the defined service.
        /// </summary>
        /// <param name="target">
        /// target name
        /// </param>
        /// <param name="service">
        /// service name
        /// </param>
        /// <param name="message">
        /// request message
        /// </param>
        /// <returns>
        /// indicates success of execution
        /// </returns>
        private bool SendRequestMessage(string target, string service, string message)
        {
            if (string.IsNullOrEmpty(target) || string.IsNullOrEmpty(message))
            {
                Logger.Warn("Invalid target ('{0}') and/or message ('{1}')", target, message);
                return false;
            }

            lock (this.httpClientLock)
            {
                var requestUriString = this.httpServerUrl + service + "/" + target;

                Logger.Debug("Request URI: {0}", requestUriString);
                try
                {
                    // Encoding is "iso-8859-1"
                    var bytesToSend = Encoding.GetEncoding("iso-8859-1").GetBytes(message);

                    // Create HTTP Web-Request
                    this.httpRequest = (HttpWebRequest)WebRequest.Create(requestUriString);

                    // HTTPSettings
                    this.httpRequest.Timeout = this.httpResponseTimeout;
                    this.httpRequest.Credentials = CredentialCache.DefaultCredentials;
                    this.httpRequest.Method = "POST";
                    this.httpRequest.KeepAlive = false;
                    this.httpRequest.ServicePoint.Expect100Continue = false;
                    this.httpRequest.ContentType = "text/xml";
                    this.httpRequest.ContentLength = bytesToSend.Length;

                    // Check whether to use a proxy
                    if (!string.IsNullOrEmpty(this.httpWebProxyHost) && (this.httpWebProxyPort > 0))
                    {
                        var webProxy = new WebProxy(this.httpWebProxyHost, this.httpWebProxyPort);

                        this.httpRequest.Proxy = webProxy;
                    }

                    Logger.Trace("GetRequestStream for data...");

                    using (var dataRequest = this.httpRequest.GetRequestStream())
                    {
                        Logger.Trace("Send data - ContentLength: {0}", this.httpRequest.ContentLength);

                        // Send HTTP request message
                        dataRequest.Write(bytesToSend, 0, bytesToSend.Length);
                        dataRequest.Flush();
                    }

                    return true;
                }
                catch (WebException e)
                {
                    var errorMessage = string.Format(
                        "SendRequestMessage WebException: Request URI: {0} Status: {1}", requestUriString, e.Status);
                    Logger.ErrorException(
                        errorMessage,
                        e);

                    this.RaiseServiceUnavailable(requestUriString);
                    throw new HttpChannelException("SendRequestMessage", e);
                }
                catch (Exception e)
                {
                    this.RaiseServiceUnavailable(requestUriString);
                    Logger.ErrorException(
                        string.Format("SendRequestMessage Exception: Request URI: {0}", requestUriString), e);
                }
            }

            return false;
        }

        /// <summary>
        /// Get HTTP response message and return the message to the caller.
        /// </summary>
        /// <returns>
        /// An object representing the response. It can be <c>null</c> if errors occurred.
        /// </returns>
        private HttpChannelResponse GetResponseMessage()
        {
            var response = new HttpChannelResponse();
            var responseUriString = string.Empty;

            try
            {
                this.httpStatus = HttpStatusCode.NoContent;

                Logger.Trace("GetResponseMessage: Wait for HTTP Server Response...");

                using (var httpWebResponse = (HttpWebResponse)this.httpRequest.GetResponse())
                {
                    this.httpStatus = httpWebResponse.StatusCode;
                    response.SetStatus(httpWebResponse);
                    Logger.LogHttpChannelResponse(response);
                    responseUriString = this.httpRequest.Address.ToString();
                    if (httpWebResponse.StatusCode != HttpStatusCode.OK)
                    {
                        Logger.Error(
                            "GetResponseMessage: HttpStatusCode: {0} StatusDescription: {1}",
                            httpWebResponse.StatusCode,
                            httpWebResponse.StatusDescription);
                        return null;
                    }

                    Logger.Debug(
                        "GetResponseMessage: Response StatusCode: {0} - StatusDescription: {1}",
                        httpWebResponse.StatusCode,
                        httpWebResponse.StatusDescription);

                    // response correctly handled, the server is available
                    this.serviceUnavailableEventRaised = false;

                    var resStream = httpWebResponse.GetResponseStream();
                    if (resStream == null)
                    {
                        return null;
                    }

                    using (var reader = new StreamReader(resStream, Encoding.GetEncoding("iso-8859-1")))
                    {
                        var message = reader.ReadToEnd();
                        response.Content = message;
                    }
                }
            }
            catch (WebException e)
            {
                Logger.ErrorException(string.Format("GetResponseMessage WebException: Status: {0}", e.Status), e);
                this.RaiseServiceUnavailable(responseUriString);
                throw new HttpChannelException("GetResponseMessage", e);
            }
            catch (Exception e)
            {
                this.RaiseServiceUnavailable(responseUriString);
                Logger.ErrorException("GetResponseMessage Exception", e);
            }

            return response;
        }

        /// <summary>
        /// Get HTTP response message and enqueue the message into the response message queue.
        /// </summary>
        /// <returns>
        /// indicates success of execution
        /// </returns>
        private bool GetResponse()
        {
            var result = false;

            // get response message
            var message = this.GetResponseMessage();

            if (!string.IsNullOrEmpty(message.Content))
            {
                // enqueue response message
                this.ResponseMessageQueue.Enqueue(message.Content);

                // raise event
                this.OnMessageReceived();

                result = true;
            }

            return result;
        }

        /// <summary>
        /// Raises message received event.
        /// </summary>
        private void OnMessageReceived()
        {
            if (this.ResponseMessageReceived != null)
            {
                Logger.Trace("OnMessageReceived Event raised.");

                this.ResponseMessageReceived(this);
            }
        }

        /// <summary>
        /// Raises listener message received event.
        /// </summary>
        /// <param name="listenerContext">
        /// HTTP listener context
        /// </param>
        private void OnListenerMessageReceived(HttpListenerContext listenerContext)
        {
            if (this.ListenerMessageReceived != null)
            {
                Logger.Trace("OnListenerMessageReceived Event raised.");

                this.ListenerMessageReceived(listenerContext);
            }
        }
    }
}