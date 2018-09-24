// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Extensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.VisualStudio.T4Directives.Utility
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Microsoft.VisualStudio.TextTemplating;

    /// <summary>
    /// Defines extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Evaluates the suggested namespace based on the current TemplateFile.
        /// </summary>
        /// <param name="host">The engine host.</param>
        /// <returns>The suggested namespace.</returns>
        public static string EvaluateSuggestedNamespace(this ITextTemplatingEngineHost host)
        {
            var fileInfo = new FileInfo(host.TemplateFile);
            var chunks = new List<string>();
            var centerFound = false;
            var directory = fileInfo.Directory;
            while (directory != null && !centerFound)
            {
                if (directory.Name == "Source")
                {
                    directory = directory.Parent;
                    continue;
                }

                if (directory.Name == "Center")
                {
                    centerFound = true;
                }

                chunks.Add(directory.Name);
                directory = directory.Parent;
            }

            chunks.Reverse();
            chunks.Insert(0, "Gorba");
            return chunks.Aggregate((s, s1) => s + "." + s1);
        }
    }
}