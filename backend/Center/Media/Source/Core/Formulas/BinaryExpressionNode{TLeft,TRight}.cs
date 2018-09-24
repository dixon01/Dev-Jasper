// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BinaryExpressionNode{TLeft,TRight}.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The BinaryExpressionNode
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Formulas
{
    using Irony.Ast;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// the binary expression node
    /// </summary>
    /// <typeparam name="TLeft">the left operant type</typeparam>
    /// <typeparam name="TRight">the right operant type</typeparam>
    public class BinaryExpressionNode<TLeft, TRight> : AstNode
        where TLeft : AstNode
        where TRight : AstNode
    {
        /// <summary>
        /// Gets the Left AST node
        /// </summary>
        public TLeft Left { get; private set; }

        /// <summary>
        /// Gets the Right AST node
        /// </summary>
        public TRight Right { get; private set; }

        /// <summary>
        /// Gets the operator node
        /// </summary>
        public ParseTreeNode OperatorNode { get; private set; }

        /// <summary>
        /// initializes the expression
        /// </summary>
        /// <param name="context">the context</param>
        /// <param name="treeNode">the tree node</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            var nodes = treeNode.GetMappedChildNodes();
            this.Left = (TLeft)this.AddChild("Arg", nodes[0]);
            this.Right = (TRight)this.AddChild("Arg", nodes[2]);
            this.OperatorNode = nodes[1];
        }
    }
}