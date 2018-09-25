// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VisualizerTransformationChain.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VisualizerTransformationChain type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Visualizer.Data.AbuDhabi
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Protran.Transformations;
    using Gorba.Common.Protocols.Isi.Messages;
    using Gorba.Motion.Protran.AbuDhabi.Transformations;
    using Gorba.Motion.Protran.Core.Transformations;

    /// <summary>
    /// Special <see cref="TransformationChain"/> to log all transformations done.
    /// </summary>
    public class VisualizerTransformationChain : TransformationChain
    {
        private readonly string name;

        private readonly List<TransformationInfo> transformationInfos = new List<TransformationInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="VisualizerTransformationChain"/> class.
        /// </summary>
        /// <param name="name">the name of this chain</param>
        /// <param name="transformations">
        /// The transformations.
        /// </param>
        public VisualizerTransformationChain(string name, IEnumerable<TransformationConfig> transformations)
            : base(transformations)
        {
            this.name = name;
            var sink = (ITransformer)this.First;
            while (sink != null)
            {
                var loggerType = typeof(TransformationLogger<>).MakeGenericType(sink.OutputType);
                var logger = (ITransformationLogger)Activator.CreateInstance(loggerType);
                logger.Transformed += (s, e) => this.transformationInfos.Add(e.Info);
                logger.Next = sink.Next;
                sink.Next = logger;
                sink = logger.Next as ITransformer;
            }
        }

        /// <summary>
        /// Event that is fired whenever an ISI Data Item was transformed.
        /// </summary>
        public event EventHandler<DataItemTransformationEventArgs> Transformed;

        /// <summary>
        /// Transforms the value with this chain.
        /// </summary>
        /// <param name="dataItem">
        /// The value.
        /// </param>
        /// <returns>
        /// the transformed value.
        /// </returns>
        public override string Transform(DataItem dataItem)
        {
            this.transformationInfos.Clear();
            var result = base.Transform(dataItem);
            var e = new DataItemTransformationEventArgs(
                dataItem, this.name, this.transformationInfos.ToArray());
            this.RaiseTransformed(e);
            return result;
        }

        private void RaiseTransformed(DataItemTransformationEventArgs e)
        {
            var handler = this.Transformed;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
