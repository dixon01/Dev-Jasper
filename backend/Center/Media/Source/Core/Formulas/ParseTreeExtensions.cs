// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParseTreeExtensions.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The ParseTreeExtensions.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Formulas
{
    using System.Linq;

    using Irony.Parsing;

    /// <summary>
    /// some extensions for the parse tree
    /// </summary>
    public static class ParseTreeExtensions
    {
        /// <summary>
        /// gets the text for the operator
        /// </summary>
        /// <param name="node">the node</param>
        /// <returns>the operator text</returns>
        public static string GetOperatorText(this ParseTreeNode node)
        {
            var child = node.ChildNodes.FirstOrDefault();
            return child != null ? child.Token.Text : node.Token.Text;
        }
    }
}