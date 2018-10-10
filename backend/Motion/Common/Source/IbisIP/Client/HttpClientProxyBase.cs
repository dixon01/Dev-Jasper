// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpClientProxyBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HttpClientProxyBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.IbisIP.Client
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Xml.Serialization;

    using Gorba.Common.Protocols.Vdv301.Messages;
    using Gorba.Common.Protocols.Vdv301.Services;
    using Gorba.Motion.Common.IbisIP.Server;

    using NLog;

    /// <summary>
    /// Base class for all IBIS-IP HTTP client proxies.
    /// </summary>
    internal abstract class HttpClientProxyBase
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly Logger Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClientProxyBase"/> class.
        /// </summary>
        /// <param name="url">
        /// The URL of the service on the IBIS-IP server.
        /// </param>
        protected HttpClientProxyBase(string url)
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName);
            this.Url = url;
        }

        /// <summary>
        /// Gets the URL of the service on the IBIS-IP server.
        /// </summary>
        protected string Url { get; private set; }

        /// <summary>
        /// Executes an HTTP GET request to the given operation.
        /// </summary>
        /// <param name="operation">
        /// The operation name (without any slashes).
        /// </param>
        /// <typeparam name="TResult">
        /// The type of object expected as a result from the remote service.
        /// </typeparam>
        /// <returns>
        /// The deserialized <see cref="TResult"/>.
        /// </returns>
        /// <exception cref="IbisIPException">
        /// If there was an error executing this request.
        /// </exception>
        protected TResult ExecuteGetRequest<TResult>(string operation)
        {
            TResult responseWrapper;
            try
            {
                var request = this.CreateWebRequest(this.Url + "/" + operation, false);

                var response = request.GetResponse();
                var responseSerializer = new XmlSerializer(typeof(TResult));
                using (var input = response.GetResponseStream())
                {
                    responseWrapper = (TResult)responseSerializer.Deserialize(input);
                }
            }
            catch (Exception ex)
            {
                throw new IbisIPException("Couldn't communicate with " + this.Url, ex);
            }

            return responseWrapper;
        }

        /// <summary>
        /// Executes an HTTP POST request to the given operation.
        /// </summary>
        /// <param name="operation">
        /// The operation name (without any slashes).
        /// </param>
        /// <param name="parameters">
        /// The parameter object to be serialized and sent to the service.
        /// </param>
        /// <typeparam name="TResult">
        /// The type of object expected as a result from the remote service.
        /// </typeparam>
        /// <returns>
        /// The deserialized <see cref="TResult"/>.
        /// </returns>
        /// <exception cref="IbisIPException">
        /// If there was an error executing this request.
        /// </exception>
        protected TResult ExecutePostRequest<TResult>(string operation, object parameters)
        {
            TResult result;
            try
            {
                var request = this.CreateWebRequest(operation, true);

                var requestSerializer = new XmlSerializer(parameters.GetType());
                using (var output = request.GetRequestStream())
                {
                    requestSerializer.Serialize(output, parameters);
                }

                var response = request.GetResponse();
                var responseSerializer = new XmlSerializer(typeof(TResult));
                using (var input = response.GetResponseStream())
                {
                    result = (TResult)responseSerializer.Deserialize(input);
                }
            }
            catch (Exception ex)
            {
                throw new IbisIPException("Couldn't communicate with " + this.Url, ex);
            }

            return result;
        }

        /// <summary>
        /// Creates a new <see cref="WebRequest"/> for the given <paramref name="operation"/>.
        /// This takes into account the <see cref="Url"/> defined when creating this proxy.
        /// </summary>
        /// <param name="operation">
        /// The operation name (without any slashes).
        /// </param>
        /// <param name="post">
        /// A flag indicating if the returned object should be a POST (true) or GET (false) request.
        /// </param>
        /// <returns>
        /// The <see cref="WebRequest"/>.
        /// </returns>
        protected WebRequest CreateWebRequest(string operation, bool post)
        {
            var request = WebRequest.Create(this.Url + "/" + operation);
            var httpMethod = post ? "POST" : "GET";
            request.Method = httpMethod;
            this.Logger.Trace("Creating {0} request: {1}", httpMethod, request.RequestUri);
            return request;
        }

        /// <summary>
        /// Called by subclasses to subscribe to the given <paramref name="operation"/>.
        /// </summary>
        /// <param name="operation">
        /// The operation name (without the "Subscribe").
        /// </param>
        /// <param name="callbackUri">
        /// The callback URI where the server should send the subscribed content to.
        /// </param>
        protected void Subscribe(string operation, Uri callbackUri)
        {
            if (callbackUri == null)
            {
                throw new ArgumentNullException("callbackUri");
            }

            ThreadPool.QueueUserWorkItem(
                s =>
                    {
                        try
                        {
                            this.DoSubscribe(operation, callbackUri);
                        }
                        catch (Exception ex)
                        {
                            this.Logger.Error(ex, "Couldn't subscribe to " + operation);
                        }
                    });
        }

        /// <summary>
        /// Called by subclasses to unsubscribe from the given <paramref name="operation"/>.
        /// </summary>
        /// <param name="operation">
        /// The operation name (without the "Unsubscribe").
        /// </param>
        /// <param name="callbackUri">
        /// The callback URI where the server was sending the subscribed content to.
        /// </param>
        protected void Unsubscribe(string operation, Uri callbackUri)
        {
            if (callbackUri == null)
            {
                return;
            }

            ThreadPool.QueueUserWorkItem(
                s =>
                {
                    try
                    {
                        this.DoUnsubscribe(operation, callbackUri);
                    }
                    catch (Exception ex)
                    {
                        this.Logger.Error(ex,"Couldn't unsubscribe from " + operation);
                    }
                });
        }

        private void DoSubscribe(string operation, Uri callbackUri)
        {
            var subscription = new SubscribeRequestStructure
                                   {
                                       ClientIPAddress = new IBISIPstring(callbackUri.Host),
                                       ReplyPort = new IBISIPint(callbackUri.Port),
                                       ReplyPath = new IBISIPstring(callbackUri.AbsolutePath)
                                   };

            var responseWrapper = this.ExecutePostRequest<SubscribeResponseStructure>(
                "Subscribe" + operation, subscription);

            var error = responseWrapper.Item as IBISIPstring;
            if (error != null)
            {
                throw new IbisIPException(error.Value);
            }

            var result = responseWrapper.Item as IBISIPboolean;
            if (result == null)
            {
                throw new IbisIPException("Returned item from " + this.Url + " was null");
            }

            if (!result.Value)
            {
                throw new IbisIPException("Couldn't subscribe to " + operation);
            }
        }

        private void DoUnsubscribe(string operation, Uri callbackUri)
        {
            var subscription = new UnsubscribeRequestStructure
                                   {
                                       ClientIPAddress = new IBISIPstring(callbackUri.Host),
                                       ReplyPort = new IBISIPint(callbackUri.Port),
                                       ReplyPath = new IBISIPstring(callbackUri.AbsolutePath)
                                   };

            var responseWrapper = this.ExecutePostRequest<UnsubscribeResponseStructure>(
                "Unsubscribe" + operation, subscription);

            var error = responseWrapper.Item as IBISIPstring;
            if (error != null)
            {
                throw new IbisIPException(error.Value);
            }

            var result = responseWrapper.Item as IBISIPboolean;
            if (result == null)
            {
                throw new IbisIPException("Returned item from " + this.Url + " was null");
            }
        }

        /// <summary>
        /// Subscription to a Get-operation on the remote server.
        /// </summary>
        /// <typeparam name="T">
        /// The type of message this subscription can handle
        /// </typeparam>
        protected class Subscription<T>
            where T : new()
        {
            private IbisHttpServer responseHandlerServer;

            /// <summary>
            /// Event that is fired whenever the server sends the subscribed content.
            /// </summary>
            public event EventHandler<DataUpdateEventArgs<T>> Updated;

            /// <summary>
            /// Gets the callback URI where the server sends the subscribed content to.
            /// </summary>
            public Uri CallbackUri { get; private set; }

            /// <summary>
            /// Starts this subscription.
            /// </summary>
            /// <param name="localServer">
            /// The local IBIS-IP HTTP server that will handle the requests.
            /// </param>
            public void Start(IbisHttpServer localServer)
            {
                if (this.CallbackUri != null)
                {
                    return;
                }

                if (localServer == null)
                {
                    throw new NotSupportedException(
                        string.Format("Can't subscribe to {0} without a local HTTP server", typeof(T).Name));
                }

                this.responseHandlerServer = localServer;
                this.CallbackUri = this.responseHandlerServer.AddSubscriptionHandler<T>(this.HandleCallback);
            }

            /// <summary>
            /// Stops this subscription.
            /// </summary>
            public void Stop()
            {
                if (this.CallbackUri == null)
                {
                    return;
                }

                this.responseHandlerServer.RemoveSubscriptionHandler(this.CallbackUri);
                this.CallbackUri = null;
            }

            /// <summary>
            /// Raises the <see cref="Updated"/> event.
            /// </summary>
            /// <param name="e">
            /// The event arguments.
            /// </param>
            protected virtual void RaiseUpdated(DataUpdateEventArgs<T> e)
            {
                var handler = this.Updated;
                if (handler != null)
                {
                    handler(this, e);
                }
            }

            private void HandleCallback(T data)
            {
                this.RaiseUpdated(new DataUpdateEventArgs<T>(data));
            }
        }
    }
}
