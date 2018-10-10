// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StartupConfigurator.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StartupConfigurator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Host
{
    using System;
    using System.Linq;
    using System.Web.Http;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Portal.Host.Middlewares;
    using Gorba.Center.Portal.Host.Services;
    using Gorba.Center.Portal.Host.Settings;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;

    using Microsoft.Owin.FileSystems;
    using Microsoft.Owin.StaticFiles;

    using NLog;

    using Owin;

    /// <summary>
    /// The startup configurator.
    /// </summary>
    public abstract class StartupConfigurator
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly Logger Logger;

        static StartupConfigurator()
        {
            Reset();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupConfigurator"/> class.
        /// </summary>
        protected StartupConfigurator()
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Gets the current configurator.
        /// </summary>
        public static StartupConfigurator Current { get; private set; }

        /// <summary>
        /// Resets the current configurator.
        /// </summary>
        public static void Reset()
        {
            Set(new DefaultStartupConfigurator());
        }

        /// <summary>
        /// Sets the current configurator.
        /// </summary>
        /// <param name="configurator">
        /// The configurator.
        /// </param>
        public static void Set(StartupConfigurator configurator)
        {
            if (configurator == null)
            {
                throw new ArgumentNullException("configurator");
            }

            Current = configurator;
        }

        /// <summary>
        /// Configures the application.
        /// </summary>
        /// <param name="appBuilder">
        /// The app builder.
        /// </param>
        public virtual void Configure(IAppBuilder appBuilder)
        {
            DependencyResolver.Current.Register<IContentResourceStorageClient>(
                new DefaultContentResourceStorageClient());
            this.ConfigureInternal(appBuilder);
        }

        /// <summary>
        /// Configures the application.
        /// </summary>
        /// <param name="appBuilder">
        /// The app builder.
        /// </param>
        protected abstract void ConfigureInternal(IAppBuilder appBuilder);

        /// <summary>
        /// Default startup configurator.
        /// </summary>
        public class DefaultStartupConfigurator : StartupConfigurator
        {
            /// <summary>
            /// Configures the application.
            /// </summary>
            /// <param name="appBuilder">
            /// The app builder.
            /// </param>
            protected override void ConfigureInternal(IAppBuilder appBuilder)
            {
                var settings = PortalSettingsProvider.Current.GetSettings();
                var appData = (IWritableDirectoryInfo)FileSystemManager.Local.GetDirectory(settings.AppDataPath);
                var staticContent = appData.GetDirectories().SingleOrDefault(info => info.Name == "StaticContent");
                if (staticContent == null)
                {
                    throw new Exception("Can't find StaticContent directory");
                }

                this.ConfigurePredefinedMiddlewares(appBuilder, staticContent, appData);
            }

            /// <summary>
            /// Configures the predefined middleware stack.
            /// </summary>
            /// <param name="appBuilder">
            /// The app builder.
            /// </param>
            /// <param name="staticContent">
            /// The static content.
            /// </param>
            /// <param name="appData">
            /// The app data.
            /// </param>
            protected virtual void ConfigurePredefinedMiddlewares(
                IAppBuilder appBuilder,
                IWritableDirectoryInfo staticContent,
                IWritableDirectoryInfo appData)
            {
                HttpConfiguration config = new HttpConfiguration();
                config.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "api/v1/{controller}",
                    defaults: new { id = RouteParameter.Optional });
                    config.MapHttpAttributeRoutes();

                appBuilder.UseWebApi(config);

                appBuilder.Use<SessionMiddleware>();
                appBuilder.Use<ConfigurationContentMiddleware>(appData);
                appBuilder.Use<HttpsRedirectMiddleware>();
                var physicalFileSystem = new PhysicalFileSystem(staticContent.FullName);
                appBuilder.UseStaticFiles(new StaticFileOptions { FileSystem = physicalFileSystem });
                appBuilder.Use<AuthenticationMiddleware>();
                appBuilder.Use<ActionMiddleware>(appData);
            }
        }
    }
}