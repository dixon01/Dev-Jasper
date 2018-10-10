// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransformationInfo.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TransformationInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Data
{
    using Gorba.Motion.Protran.Core.Transformations;

    /// <summary>
    /// Information about a step in the telegram transformation process.
    /// </summary>
    public class TransformationInfo
    {
        /// <summary>
        /// Gets or sets the input of the step.
        /// </summary>
        public object Input { get; set; }

        /// <summary>
        /// Gets or sets the transformer used in this step.
        /// </summary>
        public ITransformer Transformer { get; set; }
    }
}
