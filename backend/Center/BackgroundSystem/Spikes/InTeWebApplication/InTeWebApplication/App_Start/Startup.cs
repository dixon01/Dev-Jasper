// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Startup type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Gorba.Center.BackgroundSystem.Spikes.InTeWebApplication;

using Microsoft.Owin;

[assembly: OwinStartup(typeof(Startup))]

namespace Gorba.Center.BackgroundSystem.Spikes.InTeWebApplication
{
    using Owin;

    /// <summary>
    /// The startup.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// The configuration.
        /// </summary>
        /// <param name="app">
        /// The app.
        /// </param>
        public void Configuration(IAppBuilder app)
        {
            // Any connection or hub wire up and configuration should go here
            app.MapSignalR();
        }
    }
}