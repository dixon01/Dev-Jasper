// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectiveEngine.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DirectiveEngine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.VisualStudio.T4Directives
{
    using System;

    using Microsoft.VisualStudio.TextTemplating;

    /// <summary>
    /// Defines an engine for directives.
    /// </summary>
    public abstract class DirectiveEngine
    {
        private const string OutputTitle = "Gorba T4 directives";

        private const string MessageFormat = "[{0}] {1}";

        private static readonly Guid PaneId = new Guid("c034aade-9a6e-4446-ab13-0384787afd29");

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectiveEngine"/> class.
        /// </summary>
        /// <param name="host">
        /// The host.
        /// </param>
        protected DirectiveEngine(ITextTemplatingEngineHost host)
        {
            this.Host = host;
        }

        /// <summary>
        /// Gets the processor name.
        /// </summary>
        public abstract string ProcessorName { get; }

        /// <summary>
        /// Gets the engine host.
        /// </summary>
        public ITextTemplatingEngineHost Host { get; private set; }

        /// <summary>
        /// Writes the start message.
        /// </summary>
        protected virtual void WriteStart()
        {
            var message = "Processing file '" + this.Host.TemplateFile + "'";
            this.WriteLine(message);
        }

        /// <summary>
        /// Writes a message.
        /// </summary>
        /// <param name="message">the message to write.</param>
        protected virtual void WriteLine(string message)
        {
        }

        /// <summary>
        /// Gets a service from the host.
        /// </summary>
        /// <typeparam name="T">The type of the service.</typeparam>
        /// <returns>The service.</returns>
        protected T GetService<T>()
        {
            var serviceProvider = this.Host as IServiceProvider;
            if (serviceProvider == null)
            {
                throw new Exception("The Host is not a ServiceProvider");
            }

            return (T)serviceProvider.GetService(typeof(T));
        }
    }
}