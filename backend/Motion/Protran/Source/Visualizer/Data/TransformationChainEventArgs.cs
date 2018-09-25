// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransformationChainEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TransformationChainEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Data
{
    using System;

    /// <summary>
    /// Event argument for a telegram transformation.
    /// </summary>
    public class TransformationChainEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransformationChainEventArgs"/> class.
        /// </summary>
        /// <param name="chainName">the name of the transformation chain.</param>
        /// <param name="transformations">The transformation steps.</param>
        public TransformationChainEventArgs(string chainName, params TransformationInfo[] transformations)
        {
            this.ChainName = chainName;
            this.Transformations = transformations;
        }

        /// <summary>
        /// Gets the transformation chain.
        /// </summary>
        public string ChainName { get; private set; }

        /// <summary>
        /// Gets the transformation steps.
        /// </summary>
        public TransformationInfo[] Transformations { get; private set; }
    }
}