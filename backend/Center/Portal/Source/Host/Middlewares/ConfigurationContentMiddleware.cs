// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationContentMiddleware.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConfigurationContentMiddleware type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Host.Middlewares
{
    using System;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Portal.Host.Configuration;
    using Gorba.Common.Update.ServiceModel.Repository;
    using Gorba.Common.Utility.Files;

    using Microsoft.Owin;

    using NLog;

    /// <summary>
    /// The configuration content middleware.
    /// Handles requests for the system configuration.
    /// </summary>
    public class ConfigurationContentMiddleware : OwinMiddleware
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationContentMiddleware"/> class.
        /// </summary>
        /// <param name="next">
        /// The next.
        /// </param>
        /// <param name="appDataDirectoryInfo">
        /// The app data directory info.
        /// </param>
        public ConfigurationContentMiddleware(OwinMiddleware next, IDirectoryInfo appDataDirectoryInfo)
            : base(next)
        {
            this.AppDataDirectoryInfo = appDataDirectoryInfo;
        }

        /// <summary>
        /// Gets the app data directory.
        /// </summary>
        public IDirectoryInfo AppDataDirectoryInfo { get; private set; }

        /// <summary>
        /// Process an individual request.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Task"/> that can be awaited.</returns>
        public override async Task Invoke(IOwinContext context)
        {
            var requestContext = context.GetRequestContext();
            Logger.Trace("Invoking ConfigurationContentMiddleware for request {0}", requestContext.Id);
            try
            {
                if (context.Request.Path.HasValue)
                {
                    switch (context.Request.Path.Value)
                    {
                        case "/Configuration":
                        case "/Configuration/":
                            await this.HandleXmlFileRequest(context, "BackgroundSystemConfiguration");
                            return;
                        case "/Repository":
                        case "/Repository/":
                            await this.HandleXmlFileRequest(context, "repository");
                            return;
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Error(exception, "Error while handling a configuration request");
                context.Response.Redirect("/Error");
                return;
            }

            await this.Next.Invoke(context);
        }

        private static void HandleError(IOwinContext context, string fileName, Exception exception = null)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            if (exception == null)
            {
                return;
            }

            Logger.Error(exception, "Error while handling the " + fileName + " request");
        }

        /// <summary>
        /// Handles a request expecting the content of an xml file to be written as output.
        /// </summary>
        /// <param name="context">The OWIN context.</param>
        /// <param name="fileName">
        /// The name of the file.
        /// </param>
        /// <returns>A <see cref="Task"/> that can be awaited.</returns>
        private async Task HandleXmlFileRequest(IOwinContext context, string fileName)
        {
            try
            {
                context.Response.ContentType = "text/xml";
                var xmlWriterSettings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true };
                XmlSerializer serializer;
                switch (fileName)
                {
                    case "BackgroundSystemConfiguration":
                        var configuration = await BackgroundSystemConfigurationProvider.Current.GetConfigurationAsync();
                        serializer = new XmlSerializer(typeof(BackgroundSystemConfiguration));
                        using (var xmlWriter = XmlWriter.Create(context.Response.Body, xmlWriterSettings))
                        {
                            serializer.Serialize(xmlWriter, configuration);
                        }

                        break;
                    case "repository":
                        var repository =
                            await PortalRepositoryConfigurationProvider.Current.GetRepositoryConfigurationAsync();
                        serializer = new XmlSerializer(typeof(RepositoryConfig));
                        using (var xmlWriter = XmlWriter.Create(context.Response.Body, xmlWriterSettings))
                        {
                            serializer.Serialize(xmlWriter, repository);
                        }

                        break;
                }

                Logger.Info("Served xml file request '{0}'", fileName);
            }
            catch (Exception exception)
            {
                HandleError(context, fileName, exception);
            }
        }
    }
}