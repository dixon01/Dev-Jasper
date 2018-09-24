// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Startup type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Host
{
    using System.IO;
    using System.Linq;

    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Portal.Host.Middlewares;
    using Gorba.Common.Utility.Files;

    using Microsoft.Owin;
    using Microsoft.Owin.FileSystems;
    using Microsoft.Owin.Security.Cookies;
    using Microsoft.Owin.StaticFiles;

    using Owin;

    /// <summary>
    /// Defines the startup for the host application.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Gets the background system configuration.
        /// </summary>
        public BackgroundSystemConfiguration BackgroundSystemConfiguration { get; private set; }

        /// <summary>
        /// Configures the application.
        /// </summary>
        /// <param name="appBuilder">The builder passed by the host.</param>
        public void Configuration(IAppBuilder appBuilder)
        {
            this.BackgroundSystemConfiguration =
                BackgroundSystemConfigurationProvider.Current.GetConfiguration();
            this.ConfigureChannelScopeFactories();
            appBuilder.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                LoginPath = new PathString("/Login"),
            });

            StartupConfigurator.Current.Configure(appBuilder);
        }

        /// <summary>
        /// Configures the <see cref="IMembershipService"/> channel scope factory.
        /// </summary>
        public void ConfigureChannelScopeFactories()
        {
            ChannelScopeFactoryUtility<IMembershipService>.ConfigureAsFunctionalService(
                this.BackgroundSystemConfiguration.FunctionalServices,
                "Membership");
            ChannelScopeFactoryUtility<IAssociationTenantUserUserRoleDataService>.ConfigureAsDataService(
                this.BackgroundSystemConfiguration.DataServices,
                "AssociationTenantUserUserRole");
        }
    }
}