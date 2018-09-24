// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwitchCallNode.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The SwitchCallNode.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Formulas
{
    using Irony.Ast;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// the switch statement
    /// </summary>
    public class SwitchCallNode : AstNode
    {
        /// <summary>
        /// Gets the switch value
        /// </summary>
        public AstNode Value { get; private set; }

        /// <summary>
        /// Gets the body
        /// </summary>
        public AstNode Body { get; private set; }

        /// <summary>
        /// Gets the default
        /// </summary>
        public AstNode Default { get; private set; }

        /// <summary>
        /// The initialize function
        /// </summary>
        /// <param name="context">the context</param>
        /// <param name="treeNode">the tree node</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            var nodes = treeNode.GetMappedChildNodes();
            this.Value = this.AddChild("Value", nodes[2]);
            if (nodes.Count <= 6)
            {
                // switch without cases
                this.Default = this.AddChild("Default", nodes[4]);
            }
            else
            {
                // with cases
                this.Default = this.AddChild("Default", nodes[6]);
                this.Body = this.AddChild("Body", nodes[4]);
            }

            this.AsString = "Switch " + this.Value.AsString;
        }
    }
}