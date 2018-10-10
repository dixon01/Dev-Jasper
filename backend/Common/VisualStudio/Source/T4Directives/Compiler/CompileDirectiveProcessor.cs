// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompileDirectiveProcessor.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CompileDirectiveProcessor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.VisualStudio.T4Directives.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Microsoft.VisualStudio.TextTemplating;

    /// <summary>
    /// Processor to compile the specified items.
    /// </summary>
    public class CompileDirectiveProcessor : DirectiveProcessor
    {
        private readonly ICollection<string> assemblies = new List<string>();

        /// <summary>
        /// Gets the engine host.
        /// </summary>
        public ITextTemplatingEngineHost Host { get; private set; }

        /// <summary>
        /// Returns a value indicating whether the directive is supported.
        /// </summary>
        /// <param name="directiveName">The name of the directive.</param>
        /// <returns><c>true</c> if the directive is supported; <c>false</c> otherwise.</returns>
        public override bool IsDirectiveSupported(string directiveName)
        {
            return string.Equals(directiveName, "Compile", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Processes the directive.
        /// </summary>
        /// <param name="directiveName">The name of the directive.</param>
        /// <param name="arguments">The arguments for the processing.</param>
        public override void ProcessDirective(string directiveName, IDictionary<string, string> arguments)
        {
            if (!string.Equals("Compile", directiveName))
            {
                throw new NotSupportedException("Only the 'Compile' directive is supported");
            }

            var engine = new CompilerDirectiveEngine(this.Host);
            var result = engine.Execute(arguments);
            if (!result.Succeeded)
            {
                throw new Exception(string.Format("Build failed! Log: {0}", result.Log));
            }

            bool unloadGenerationAppDomain;
            if (arguments.ContainsKey("UnloadGenerationAppDomain")
                && bool.TryParse(arguments["UnloadGenerationAppDomain"], out unloadGenerationAppDomain)
                && unloadGenerationAppDomain)
            {
                this.UnloadGenerationAppDomain();
            }

            result.Output.ToList().ForEach(this.assemblies.Add);
        }

        /// <summary>
        /// Initializes this processor.
        /// </summary>
        /// <param name="host">
        /// The host.
        /// </param>
        public override void Initialize(ITextTemplatingEngineHost host)
        {
            this.Host = host;
        }

        /// <summary>
        /// Finishes the processing.
        /// </summary>
        public override void FinishProcessingRun()
        {
            // nothing to do here
        }

        /// <summary>
        /// Gets the class code.
        /// </summary>
        /// <returns>The class code.</returns>
        public override string GetClassCodeForProcessingRun()
        {
            return string.Empty;
        }

        /// <summary>
        /// Pre-initialization for processing.
        /// </summary>
        /// <returns>The initialization code string.</returns>
        public override string GetPreInitializationCodeForProcessingRun()
        {
            return string.Empty;
        }

        /// <summary>
        /// Post-initialization for processing.
        /// </summary>
        /// <returns>The initialization code string.</returns>
        public override string GetPostInitializationCodeForProcessingRun()
        {
            return string.Empty;
        }

        /// <summary>
        /// Gets references for processing.
        /// </summary>
        /// <returns>the reference assemblies.</returns>
        public override string[] GetReferencesForProcessingRun()
        {
            return this.assemblies.ToArray();
        }

        /// <summary>
        /// Gets the imports for processing.
        /// </summary>
        /// <returns>The imports for processing.</returns>
        public override string[] GetImportsForProcessingRun()
        {
            return new string[0];
        }

        /// <summary>
        /// <see cref="http://social.msdn.microsoft.com/Forums/vstudio/en-US/b113fd68-d217-432f-8ce8-bce9f28f74e2/
        /// dsl-t4-forcing-new-appdomain-for-template-generation?forum=vsx"/>
        /// </summary>
        private void UnloadGenerationAppDomain()
        {
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