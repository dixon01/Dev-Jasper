// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImportProjectDirectiveProcessor.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImportProjectDirectiveProcessor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.VisualStudio.T4Directives.ImportProject
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Processor for the ImportProject directive.
    /// </summary>
    public class ImportProjectDirectiveProcessor : DirectiveProcessorBase
    {
        private readonly ICollection<string> assemblies = new List<string>();

        /// <inheritdoc />
        public override bool IsDirectiveSupported(string directiveName)
        {
            return string.Equals(directiveName, "ImportProject", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <inheritdoc />
        public override void ProcessDirective(string directiveName, IDictionary<string, string> arguments)
        {
            if (!string.Equals("ImportProject", directiveName))
            {
                throw new NotSupportedException("Only the 'ImportProject' directive is supported");
            }

            var engine = new ImportProjectDirectiveEngine(this.Host);
            var result = engine.Execute(arguments);
            if (!result.Succeeded)
            {
                throw new Exception(string.Format("Build failed! Log: {0}", result.Log));
            }

            this.UnloadGenerationAppDomain(arguments);
            result.Output.ToList().ForEach(this.assemblies.Add);
        }

        /// <inheritdoc />
        public override void FinishProcessingRun()
        {
            // nothing to do here
        }

        /// <inheritdoc />
        public override string GetClassCodeForProcessingRun()
        {
            return string.Empty;
        }

        /// <inheritdoc />
        public override string GetPreInitializationCodeForProcessingRun()
        {
            return string.Empty;
        }

        /// <inheritdoc />
        public override string GetPostInitializationCodeForProcessingRun()
        {
            return string.Empty;
        }

        /// <inheritdoc />
        public override string[] GetReferencesForProcessingRun()
        {
            return this.assemblies.ToArray();
        }

        /// <inheritdoc />
        public override string[] GetImportsForProcessingRun()
        {
            return new string[0];
        }
    }
}