// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DirectiveProcessorBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DirectiveProcessorBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.VisualStudio.T4Directives
{
    using System.Collections.Generic;
    using System.Reflection;

    using Microsoft.VisualStudio.TextTemplating;

    /// <summary>
    /// Defines a base class for directive processors.
    /// </summary>
    public abstract class DirectiveProcessorBase : DirectiveProcessor
    {
        /// <summary>
        /// Gets the host.
        /// </summary>
        public ITextTemplatingEngineHost Host { get; private set; }

        /// <summary>
        /// Initializes the host.
        /// </summary>
        /// <param name="host">
        /// The host.
        /// </param>
        public override void Initialize(ITextTemplatingEngineHost host)
        {
            this.Host = host;
        }

        /// <summary>
        /// <see cref="http://social.msdn.microsoft.com/Forums/vstudio/en-US/b113fd68-d217-432f-8ce8-bce9f28f74e2/
        /// dsl-t4-forcing-new-appdomain-for-template-generation?forum=vsx"/>
        /// </summary>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        protected void UnloadGenerationAppDomain(IDictionary<string, string> arguments)
        {
            bool unloadGenerationAppDomain;
            if (!arguments.ContainsKey("UnloadGenerationAppDomain")
                || !bool.TryParse(arguments["UnloadGenerationAppDomain"], out unloadGenerationAppDomain)
                || !unloadGenerationAppDomain)
            {
                return;
            }

            // HACK!: Reflection hack for now to force the unloading of the code-gen AppDomain every time we need to
            // load a new version of the assembly
            var method = this.Host.GetType()
                .GetMethod("UnloadGenerationAppDomain", BindingFlags.Instance | BindingFlags.NonPublic);
            if (method == null)
            {
                return;
            }

            method.Invoke(this.Host, new object[] { });
        }
    }
}