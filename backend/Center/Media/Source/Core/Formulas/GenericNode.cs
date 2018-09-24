// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericNode.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The GenericNode.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Formulas
{
    using System.Linq;

    using Irony.Ast;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// the Generic node
    /// </summary>
    public class GenericNode : AstNode
    {
        /// <summary>
        /// Gets the table
        /// </summary>
        public string Table { get; private set; }

        /// <summary>
        /// Gets the column
        /// </summary>
        public string Column { get; private set; }

        /// <summary>
        /// Gets the row
        /// </summary>
        public string Row { get; private set; }

        /// <summary>
        /// Gets the language
        /// </summary>
        public string Language { get; private set; }

        /// <summary>
        /// The initialize function
        /// </summary>
        /// <param name="context">the context</param>
        /// <param name="treeNode">the tree node</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            var nodes = treeNode.GetMappedChildNodes();

            this.Table = nodes[1].Token.Text;
            this.Column = nodes[3].Token.Text;

            this.AsString = string.Format("${0}.{1}", this.Table, this.Column);

            if (nodes.Count > 4)
            {
                this.Row = this.GetOptionalNumericChild(nodes[4]);

                this.AsString += "[" + this.Row + "]";
            }

            if (nodes.Count > 5)
            {
                this.Language = this.GetOptionalNumericChild(nodes[5]);

                this.AsString += "{" + this.Language + "}";
            }
        }

        private string GetOptionalNumericChild(ParseTreeNode parseTreeNode)
        {
            string result = null;

            if (parseTreeNode.ChildNodes.Any())
            {
                var childNodesOfChildNodes = parseTreeNode.ChildNodes[0].ChildNodes;
                if (childNodesOfChildNodes.Any())
                {
                    if (childNodesOfChildNodes[0].ChildNodes.Count > 0)
                    {
                        result = childNodesOfChildNodes[0].ChildNodes[1].Token.Text;
                    }
                }
            }

            return result;
        }
    }
}