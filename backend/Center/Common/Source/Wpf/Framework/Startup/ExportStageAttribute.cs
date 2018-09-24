// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportStageAttribute.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ExportStageAttribute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Startup
{
    using System;
    using System.ComponentModel.Composition;

    using Gorba.Center.Common.Wpf.Framework.ViewModels;

    /// <summary>
    /// Exports a stage with a defined index for sorting.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    [MetadataAttribute]
    public sealed class ExportStageAttribute : ExportAttribute, IStageMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportStageAttribute"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        public ExportStageAttribute(int index = 0)
            : base(typeof(IStage))
        {
            this.Index = index;
        }

        /// <summary>
        /// Gets or sets the data scope associated to the stage.
        /// </summary>
        /// <value>
        /// The data scope associated to the stage.
        /// </value>
        public string DataScope { get; set; }

        /// <summary>
        /// Gets the index in the displayed list of stages.
        /// </summary>
        public int Index { get; private set; }
    }
}
