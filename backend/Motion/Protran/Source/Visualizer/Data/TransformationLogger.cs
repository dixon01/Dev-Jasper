// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransformationLogger.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TransformationLogger type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Data
{
    using System;

    using Gorba.Common.Configuration.Protran.Transformations;
    using Gorba.Motion.Protran.Core.Transformations;

    /// <summary>
    /// Transformation log class that can log a single transformation step.
    /// </summary>
    /// <typeparam name="T">
    /// The input/output type of this transformer.
    /// </typeparam>
    public class TransformationLogger<T> : Transformer<T, T, TransformationConfig>, ITransformationLogger
    {
        /// <summary>
        /// Event that is fired when this transformer has
        /// done a transformation.
        /// </summary>
        public event EventHandler<TransformationEventArgs> Transformed;

        /// <summary>
        /// Actual transformation method to be implemented by subclasses.
        /// </summary>
        /// <param name="value">the value to be transformed.</param>
        /// <returns>the transformed value.</returns>
        protected override T DoTransform(T value)
        {
            var info = new TransformationInfo
                { Input = value, Transformer = this.Next as ITransformer };
            this.RaiseTransformed(new TransformationEventArgs(info));
            if (info.Transformer != null && info.Transformer.Config != null)
            {
                // force reconfiguration to update possibly modified config
                info.Transformer.Configure(info.Transformer.Config);
            }

            return value;
        }

        private void RaiseTransformed(TransformationEventArgs e)
        {
            var handler = this.Transformed;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}