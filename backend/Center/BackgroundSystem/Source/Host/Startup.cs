// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Startup type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Host
{
    using System.Web.Http;

    using Owin;

    /// <summary>
    /// The startup configuration for Api controllers.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// This code configures Web API. The Startup class is specified as a type parameter in the WebApp.Start method.
        /// </summary>
        /// <param name="appBuilder">
        /// The app builder.
        /// </param>
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host.
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
                config.MapHttpAttributeRoutes();

            appBuilder.UseWebApi(config);
        }
    }
}