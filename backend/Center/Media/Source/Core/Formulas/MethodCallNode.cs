// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodCallNode.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The MethodCallNode.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Formulas
{
    using Irony.Ast;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// the method call
    /// </summary>
    public class MethodCallNode : AstNode
    {
        private AstNode targetRef;
        private AstNode arguments;
        private string targetName;

        /// <summary>
        /// Gets the target name of the method
        /// </summary>
        public string TargetName
        {
            get
            {
                return this.targetName;
            }
        }

        /// <summary>
        /// Gets the arguments
        /// </summary>
        public AstNode Arguments
        {
            get
            {
                return this.arguments;
            }
        }

        /// <summary>
        /// The initialize function
        /// </summary>
        /// <param name="context">the context</param>
        /// <param name="treeNode">the tree node</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            var nodes = treeNode.GetMappedChildNodes();
            this.targetRef = this.AddChild("Target", nodes[0]);
            this.targetRef.UseType = NodeUseType.CallTarget;
            this.targetName = nodes[0].FindTokenAndGetText();
            this.arguments = this.AddChild("Args", nodes[2]);
            this.AsString = "Call " + this.targetName;
        }
    }
}