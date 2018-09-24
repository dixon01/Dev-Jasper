// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluralizationServiceDirectiveProcessor.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PluralizationServiceDirectiveProcessor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.VisualStudio.T4Directives
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the processor for the pluralization service.
    /// </summary>
    public class PluralizationServiceDirectiveProcessor : DirectiveProcessorBase
    {
        private const string DirectiveName = "PluralizationService";

        private const string Code = @"private PluralizationService pluralizationService = PluralizationService.
CreateService(System.Globalization.CultureInfo.GetCultureInfo(""en-us""));";

        /// <inheritdoc />
        public override bool IsDirectiveSupported(string directiveName)
        {
            return string.Equals("Pluralizer", directiveName);
        }

        /// <inheritdoc />
        public override void ProcessDirective(string directiveName, IDictionary<string, string> arguments)
        {
            if (!string.Equals(DirectiveName, directiveName))
            {
                throw new NotSupportedException("Only the 'EntitySpace' directive is supported");
            }

            this.UnloadGenerationAppDomain(arguments);
        }

        /// <inheritdoc />
        public override void FinishProcessingRun()
        {
        }

        /// <inheritdoc />
        public override string GetClassCodeForProcessingRun()
        {
            return Code;
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
            return new[] { "System.Data.Entity.Design" };
        }

        /// <inheritdoc />
        public override string[] GetImportsForProcessingRun()
        {
            return new[] { "System.Data.Entity.Design.PluralizationServices" };
        }
    }
}