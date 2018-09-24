// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransformationEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TransformationEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Data
{
    using System;

    /// <summary>
    /// Event arguments for a single transformation step.
    /// </summary>
    public class TransformationEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransformationEventArgs"/> class.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        public TransformationEventArgs(TransformationInfo info)
        {
            this.Info = info;
        }

        /// <summary>
        /// Gets or sets transformation info.
        /// </summary>
        public TransformationInfo Info { get; set; }
    }
}
