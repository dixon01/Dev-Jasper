// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisHttpServer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisHttpServer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.IbisIP.Server
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Xml.Schema;

    using Gorba.Common.Protocols.Vdv301.Services;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.Common.IbisIP.Client;
    using Gorba.Motion.Common.IbisIP.Schema;

    using NLog;

    /// <summary>
    /// The HTTP server that handles IBIS-IP requests.
    /// </summary>
    public class IbisHttpServer : HttpServer
    {
        private const string IndexPage = "index.html";
        private const string PostPage = "post.html";

        private static readonly Logger Logger = LogHelper.GetLogger<IbisHttpServer>();

        private static volatile XmlSchemaSet xsdSchemas;

        private readonly Dictionary<string, HttpRequestHandlerBase> handlers;

        /// <summary>
        /// Initializes a new instance of the <see cref="IbisHttpServer"/> class.
        /// </summary>
        /// <param name="endPoint">
        /// The local end point to bind the server to.
        /// </param>
        public IbisHttpServer(IPEndPoint endPoint)
            : base(endPoint)
        {
            this.handlers = new Dictionary<string, HttpRequestHandlerBase>();
        }

        /// <summary>
        /// Gets or sets a value indicating whether to validate the XML structure of received HTTP requests.
        /// </summary>
        public bool ValidateRequests { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to validate the XML structure of created HTTP responses.
        /// </summary>
        public bool ValidateResponses { get; set; }

        private static XmlSchemaSet XsdSchemas
        {
            get
            {
                if (xsdSchemas == null)
                {
                    lock (typeof(IbisHttpServer))
                    {
                        if (xsdSchemas == null)
                        {
                            xsdSchemas = SchemaSetFactory.LoadSchemaSet();
                        }
                    }
                }

                return xsdSchemas;
            }
        }

        /// <summary>
        /// Adds a service to this server.
        /// </summary>
        /// <typeparam name="TService">
        /// The type of the service.
        /// It must be an <see cref="IVdv301HttpService"/> as well as implementing
        /// <see cref="IVdv301ServiceImpl"/>.
        /// </typeparam>
        /// <param name="service">
        /// The service.
        /// </param>
        /// <returns>
        /// The path to the service (without the service name).
        /// </returns>
        public string AddService<TService>(TService service)
            where TService : IVdv301HttpService
        {
            var handler = HttpServiceHandlerBase.Create(service);
            this.AddHandler(handler);
            return Definitions.CurrentVersion.ToString();
        }

        /// <summary>
        /// Adds a subscription handler.
        /// This is only used by <see cref="HttpClientProxyBase.Subscription{T}"/> to register for callback URLs.
        /// </summary>
        /// <param name="callback">
        /// The callback called when the remote server calls this handler.
        /// </param>
        /// <typeparam name="T">
        /// The type of message expected from the remote server.
        /// </typeparam>
        /// <returns>
        /// The <see cref="Uri"/> to which the <paramref name="callback"/> is registered.
        /// </returns>
        internal Uri AddSubscriptionHandler<T>(Action<T> callback)
            where T : new()
        {
            var handler = new SubscriptionHandler<T>(callback);
            this.AddHandler(handler);

            var addresses = this.LocalAddresses;
            if (addresses.Length == 0)
            {
                throw new NotSupportedException("Couldn't determine local IP address");
            }

            // TODO: how do we figure out the right callback IP address to return?
            var uri =
                new Uri(
                    string.Format(
                        "http://{0}:{1}/{2}/{3}/Callback",
                        addresses[0],
                        this.LocalPort,
                        Definitions.CurrentVersion,
                        handler.Name));
            Logger.Debug("Created subscription callback: {0}", uri);
            return uri;
        }

        /// <summary>
        /// Removes a subscription handler previously added with <see cref="AddSubscriptionHandler{T}"/>.
        /// </summary>
        /// <param name="callbackUri">
        /// The callback URI returned from <see cref="AddSubscriptionHandler{T}"/>.
        /// </param>
        internal void RemoveSubscriptionHandler(Uri callbackUri)
        {
            var pathParts = callbackUri.AbsolutePath.Split('/');
            this.handlers.Remove(pathParts[2]);
        }

        /// <summary>
        /// Method to be implemented by subclasses to handle a new HTTP request.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        protected override void HandleRequest(HttpServer.Request request)
        {
            var pathParts = request.Path.Trim('/').Split('/');
            if (pathParts.Length == 0)
            {
                throw new FileNotFoundException("File not found: " + request.Path);
            }

            try
            {
                var version = new Version(pathParts[0]);
                if (!version.Equals(Definitions.CurrentVersion))
                {
                    throw new FileNotFoundException("File not found: " + request.Path);
                }
            }
            catch (Exception)
            {
                // CF 3.5 doesn't have a constructor with an exception as argument
                throw new FileNotFoundException("File not found: " + request.Path);
            }

            if (pathParts.Length == 1)
            {
                this.HandleServiceListRequest(request);
                return;
            }

            HttpRequestHandlerBase handler;
            if (!this.handlers.TryGetValue(pathParts[1], out handler))
            {
                throw new FileNotFoundException("File not found: " + request.Path);
            }

            if (pathParts.Length == 2 || pathParts[2] == IndexPage)
            {
                handler.HandleListRequest(request);
                return;
            }

            if (pathParts[2].StartsWith(PostPage))
            {
                var postParts = pathParts[2].Split('?');
                if (postParts.Length != 2)
                {
                    throw new ArgumentException("No operation specified");
                }

                handler.HandlePostFormRequest(postParts[1], request);
                return;
            }

            handler.HandleRequest(pathParts[2], request);
        }

        private void AddHandler(HttpRequestHandlerBase handler)
        {
            if (this.ValidateRequests || this.ValidateResponses)
            {
                handler.SetValidation(XsdSchemas, this.ValidateRequests, this.ValidateResponses);
            }

            this.handlers.Add(handler.Name, handler);
        }

        private void HandleServiceListRequest(Request request)
        {
            using (var writer = new StreamWriter(request.GetResponse().GetResponseStream()))
            {
                writer.WriteLine("<html>");
                writer.WriteLine("<head><title>IBIS-IP HTTP Server</title></head>");
                writer.WriteLine("<body>");
                writer.WriteLine("<h1>IBIS-IP HTTP Server</h1>");
                writer.WriteLine("<h2>Registered Services</h2>");
                writer.WriteLine("<ul>");
                foreach (var service in this.handlers)
                {
                    if (service.Value is HttpServiceHandlerBase)
                    {
                        writer.WriteLine(
                            "<li><a href=\"/{0}/{1}/{2}\">{1}</a></li>",
                            Definitions.CurrentVersion,
                            service.Key,
                            IndexPage);
                    }
                }

                writer.WriteLine("</ul>");
                writer.WriteLine("<h2>Registered Callbacks</h2>");
                writer.WriteLine("<ul>");
                foreach (var service in this.handlers)
                {
                    if (!(service.Value is HttpServiceHandlerBase))
                    {
                        writer.WriteLine(
                            "<li><a href=\"/{0}/{1}/{2}?Callback\">{1}</a></li>",
                            Definitions.CurrentVersion,
                            service.Key,
                            PostPage);
                    }
                }

                writer.WriteLine("</ul>");
                writer.WriteLine("</body>");
                writer.WriteLine("</html>");
            }
        }
    }
}
