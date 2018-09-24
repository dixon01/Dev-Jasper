// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpServiceHandlerBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HttpServiceHandlerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.IbisIP.Server
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Threading;

    using Gorba.Common.Protocols.Vdv301.Messages;

    /// <summary>
    /// The base class for all generated HTTP service handlers.
    /// </summary>
    internal abstract partial class HttpServiceHandlerBase : HttpRequestHandlerBase
    {
        /// <summary>
        /// Adds a subscription to the given list and answers the given HTTP request.
        /// </summary>
        /// <typeparam name="T">
        /// The type of message expected from the remote server.
        /// </typeparam>
        /// <param name="subscriptions">
        /// The subscription list to add the subscription to.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="Subscription{T}"/> added to the list or null if none was added.
        /// </returns>
        protected Subscription<T> AddSubscription<T>(List<Subscription<T>> subscriptions, HttpServer.Request request)
        {
            Subscription<T> subscription;
            var resultWrapper = new SubscribeResponseStructure();
            try
            {
                if (request.Method != "POST")
                {
                    throw new NotSupportedException("Subscribe only supports POST");
                }

                SubscribeRequestStructure args;
                using (var input = request.GetRequestStream())
                {
                    args = this.Deserialize<SubscribeRequestStructure>(input);
                }

                subscription = new Subscription<T>(args, this);
                subscriptions.Add(subscription);

                resultWrapper.Item = new IBISIPboolean(true);
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Exception while dispatching Subscribe, returning error");
                resultWrapper.Item = new IBISIPstring
                                         {
                                             Value = ex.GetType().Name + ": " + ex.Message
                                         };
                subscription = null;
            }

            var response = request.GetResponse();
            response.ContentType = "text/xml";
            using (var output = response.GetResponseStream())
            {
                this.Serialize(output, resultWrapper);
            }

            return subscription;
        }

        /// <summary>
        /// Removes a subscription from the given list and answers the given HTTP request.
        /// </summary>
        /// <typeparam name="T">
        /// The type of message expected from the remote server.
        /// </typeparam>
        /// <param name="subscriptions">
        /// The subscription list to remove the subscription from.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        protected void RemoveSubscription<T>(List<Subscription<T>> subscriptions, HttpServer.Request request)
        {
            var resultWrapper = new UnsubscribeResponseStructure();
            try
            {
                if (request.Method != "POST")
                {
                    throw new NotSupportedException("Unsubscribe only supports POST");
                }

                UnsubscribeRequestStructure args;
                using (var input = request.GetRequestStream())
                {
                    args = this.Deserialize<UnsubscribeRequestStructure>(input);
                }

                foreach (var subscription in subscriptions)
                {
                    if (subscription.Request.ClientIPAddress.Value == args.ClientIPAddress.Value
                        && subscription.Request.ReplyPath.Value == args.ReplyPath.Value
                        && subscription.Request.ReplyPort.Value == args.ReplyPort.Value)
                    {
                        subscriptions.Remove(subscription);
                        break;
                    }
                }

                resultWrapper.Item = new IBISIPboolean(false);
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Exception while dispatching Unsubscribe, returning error");
                resultWrapper.Item = new IBISIPstring
                                         {
                                             Value = ex.GetType().Name + ": " + ex.Message
                                         };
            }

            var response = request.GetResponse();
            response.ContentType = "text/xml";
            using (var output = response.GetResponseStream())
            {
                this.Serialize(output, resultWrapper);
            }
        }

        /// <summary>
        /// Notifies all subscriptions with the new value.
        /// </summary>
        /// <typeparam name="T">
        /// The type of message expected from the remote server.
        /// </typeparam>
        /// <param name="subscriptions">
        /// The subscriptions.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        protected void NotifySubscriptions<T>(List<Subscription<T>> subscriptions, T value)
        {
            foreach (var subscription in subscriptions)
            {
                var sub = subscription;
                ThreadPool.QueueUserWorkItem(
                    s =>
                        {
                            try
                            {
                                sub.Notify(value);
                            }
                            catch (Exception ex)
                            {
                                this.Logger.Warn(ex, "Couldn't notify " + typeof(T).Name);
                            }
                        });
            }
        }

        /// <summary>
        /// An object encapsulating a subscription request.
        /// </summary>
        /// <typeparam name="T">
        /// The type of message expected from the remote server.
        /// </typeparam>
        protected class Subscription<T>
        {
            private readonly HttpServiceHandlerBase owner;

            /// <summary>
            /// Initializes a new instance of the <see cref="Subscription{T}"/> class.
            /// </summary>
            /// <param name="request">
            /// The request.
            /// </param>
            /// <param name="owner">
            /// The handler to which this subscription belongs.
            /// </param>
            public Subscription(SubscribeRequestStructure request, HttpServiceHandlerBase owner)
            {
                this.owner = owner;
                this.Request = request;
            }

            /// <summary>
            /// Gets the request.
            /// </summary>
            public SubscribeRequestStructure Request { get; private set; }

            /// <summary>
            /// Notifies the subscription.
            /// </summary>
            /// <param name="value">
            /// The value.
            /// </param>
            public void Notify(T value)
            {
                var path = this.Request.ReplyPath.Value;
                if (path.StartsWith("/"))
                {
                    path = path.Substring(1);
                }

                var url = string.Format(
                    "http://{0}:{1}/{2}", this.Request.ClientIPAddress.Value, this.Request.ReplyPort.Value, path);
                try
                {
                    var request = WebRequest.Create(url);
                    request.Method = "POST";

                    using (var output = request.GetRequestStream())
                    {
                        this.owner.Serialize(output, value);
                    }

                    var response = request.GetResponse();
                    var input = response.GetResponseStream();
                    if (input != null)
                    {
                        // we can ignore the return value of a callback
                        input.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw new IbisIPException("Couldn't communicate with " + url, ex);
                }
            }
        }
    }
}