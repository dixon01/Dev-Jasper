// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApiHost.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Host
{
    using System;

    using Gorba.Center.BackgroundSystem.Core;

    using Microsoft.Owin.Hosting;

    /// <summary>
    /// A host for web api.
    /// </summary>
    /// <typeparam name="T">The type of the startup configuration class.</typeparam>
    public class ApiHost<T> : IServiceHost
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiHost{T}"/> class.
        /// </summary>
        /// <param name="name">The name of the Api host.</param>
        /// <param name="uri">The uri.</param>
        public ApiHost(string name, string uri)
        {
            this.Name = name;
            this.Uri = uri;
        }

        /// <summary>
        /// Gets the uri.
        /// </summary>
        public string Uri { get; private set; }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        public object Configuration { get; set; }

        /// <summary>
        /// Gets the app.
        /// </summary>
        public IDisposable App { get; private set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// The open.
        /// </summary>
        public void Open()
        {
            this.App = WebApp.Start<T>(this.Uri);
        }

        /// <summary>
        /// The close.
        /// </summary>
        public void Close()
        {
            if (this.App == null)
            {
                return;
            }

            this.App.Dispose();
        }
    }
}