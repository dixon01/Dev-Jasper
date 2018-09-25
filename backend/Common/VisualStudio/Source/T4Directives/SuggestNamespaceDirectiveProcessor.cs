// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SuggestNamespaceDirectiveProcessor.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SuggestNamespaceDirectiveProcessor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.VisualStudio.T4Directives
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.VisualStudio.T4Directives.Utility;

    /// <summary>
    /// Adds a variable containing the suggested namespace.
    /// </summary>
    public class SuggestNamespaceDirectiveProcessor : DirectiveProcessorBase
    {
        private const string Code = @"private string {0} =
string.IsNullOrEmpty(System.Runtime.Remoting.Messaging.CallContext.LogicalGetData(""NamespaceHint"") as string) ?
""{1}"" : System.Runtime.Remoting.Messaging.CallContext.LogicalGetData(""NamespaceHint"") as string;";

        private string variableName;

        /// <inheritdoc />
        public override bool IsDirectiveSupported(string directiveName)
        {
            return string.Equals("SuggestNamespace", directiveName);
        }

        /// <inheritdoc />
        public override void ProcessDirective(string directiveName, IDictionary<string, string> arguments)
        {
            if (!string.Equals("SuggestNamespace", directiveName))
            {
                throw new NotSupportedException("Only the 'SuggestNamespace' directive is supported");
            }

            this.variableName = arguments
                .ContainsKey("VariableName") ? arguments["VariableName"] : "suggestedNamespace";
        }

        /// <inheritdoc />
        public override void FinishProcessingRun()
        {
        }

        /// <inheritdoc />
        public override string GetClassCodeForProcessingRun()
        {
            var defaultNamespace = this.Host.EvaluateSuggestedNamespace();
            return string.Format(Code, this.variableName, defaultNamespace);
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
            return new string[0];
        }

        /// <inheritdoc />
        public override string[] GetImportsForProcessingRun()
        {
            return new string[0];
        }
    }
}