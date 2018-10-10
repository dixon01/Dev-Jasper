// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StarExpressionNode.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The StarExpressionNode
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Formulas
{
    using Irony.Ast;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// the star expression node
    /// </summary>
    public class StarExpressionNode : AstNode
    {
        /// <summary>
        /// Gets the Expression
        /// </summary>
        public AstNode Expression { get; private set; }

        /// <summary>
        /// initializes the expression
        /// </summary>
        /// <param name="context">the context</param>
        /// <param name="treeNode">the tree node</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            var nodes = treeNode.GetMappedChildNodes();
            this.Expression = this.AddChild("Expression", nodes[1]);
        }
    }
}