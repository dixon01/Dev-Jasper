// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SubscriptionHandler.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SubscriptionHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.IbisIP.Server
{
    using System;
    using System.IO;
    using System.Threading;

    using Gorba.Common.Protocols.Vdv301.Messages;

    /// <summary>
    /// Handler for subscriptions in the IBIS-IP HTTP server.
    /// </summary>
    /// <typeparam name="T">
    /// The type of message expected from the remote server.
    /// </typeparam>
    internal class SubscriptionHandler<T> : HttpRequestHandlerBase
        where T : new()
    {
        // ReSharper disable StaticFieldInGenericType
        private static int subscriptionCounter;

        // ReSharper restore StaticFieldInGenericType
        private readonly Action<T> callback;

        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="SubscriptionHandler{T}"/> class.
        /// </summary>
        /// <param name="callback">
        /// The callback called when the remote server calls this handler.
        /// </param>
        public SubscriptionHandler(Action<T> callback)
        {
            this.callback = callback;
            this.name = string.Format("{0}-{1}", typeof(T).Name, Interlocked.Increment(ref subscriptionCounter));
        }

        /// <summary>
        /// Gets the name of the service.
        /// </summary>
        public override string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Handles the request to list services as HTML.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        public override void HandleListRequest(HttpServer.Request request)
        {
            // the list request is not supported
            throw new FileNotFoundException("File not found: " + request.Path);
        }

        /// <summary>
        /// Handles the request to execute an operation.
        /// </summary>
        /// <param name="operationName">
        /// The operation name.
        /// </param>
        /// <param name="request">
        /// The request.
        /// </param>
        public override void HandleRequest(string operationName, HttpServer.Request request)
        {
            if (request.Method != "POST")
            {
                throw new NotSupportedException("Subscription handlers only support POST");
            }

            T args;
            using (var input = request.GetRequestStream())
            {
                args = this.Deserialize<T>(input);
            }

            var resultWrapper = new DataAcceptedResponseStructure();
            try
            {
                this.callback(args);
                resultWrapper.Item = new DataAcceptedResponseDataStructure { DataAccepted = new IBISIPboolean(true) };
            }
            catch (Exception ex)
            {
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
        /// Gets the default object to be shown in the POST form for an operation.
        /// </summary>
        /// <param name="operationName">
        /// The operation name.
        /// </param>
        /// <returns>
        /// The <see cref="object"/> that will be XML serialized.
        /// </returns>
        protected override object GetDefaultPostObject(string operationName)
        {
            return this.CreateDefaultPostObject<T>();
        }
    }
}