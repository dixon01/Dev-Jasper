// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvMappingCallNode.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The CsvMappingCallNode.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Formulas
{
    using Irony.Ast;
    using Irony.Interpreter.Ast;
    using Irony.Parsing;

    /// <summary>
    /// the csv mapping call
    /// </summary>
    public class CsvMappingCallNode : AstNode
    {
        /// <summary>
        /// Gets the file name value
        /// </summary>
        public AstNode FileName { get; private set; }

        /// <summary>
        /// Gets the output format value
        /// </summary>
        public AstNode Format { get; private set; }

        /// <summary>
        /// Gets the default
        /// </summary>
        public AstNode Default { get; private set; }

        /// <summary>
        /// Gets the body
        /// </summary>
        public AstNode Body { get; private set; }

        /// <summary>
        /// The initialize function
        /// </summary>
        /// <param name="context">the context</param>
        /// <param name="treeNode">the tree node</param>
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);

            var nodes = treeNode.GetMappedChildNodes();
            this.FileName = this.AddChild("FileName", nodes[2]);
            this.Format = this.AddChild("Format", nodes[4]);
            this.Default = this.AddChild("Default", nodes[6]);
            this.Body = this.AddChild("Body", nodes[8]);

            this.AsString = "CsvMapping " + this.FileName.AsString;
        }
    }
}