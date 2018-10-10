// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransformationChain.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   A chain of transformers that transform a string.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.SoapIbisPlus.Service
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Protran.Transformations;
    using Gorba.Motion.Protran.Core.Transformations;

    /// <summary>
    /// A chain of transformers that transform a string.
    /// </summary>
    public class TransformationChain
    {
        private readonly TransformationSink sink = new TransformationSink();

        private readonly ITransformationSink<string> first;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformationChain"/> class.
        /// </summary>
        /// <param name="transformations">
        /// The transformation configurations.
        /// </param>
        public TransformationChain(IEnumerable<TransformationConfig> transformations)
        {
            var initial = new TransformationSource();
            this.first = initial;
            ITransformer previous = initial;
            foreach (var transcoderConfig in transformations)
            {
                var current = TransformerFactory.CreateTransformer(transcoderConfig, previous.OutputType);
                previous.Next = current;
                previous = current;
            }

            previous.Next = this.sink;
        }

        /// <summary>
        /// Transforms the value with this chain.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// the transformed value.
        /// </returns>
        public virtual string Transform(string value)
        {
            this.first.Transform(value);
            return this.sink.Value;
        }

        /// <summary>
        /// Source transformer for the chain.
        /// </summary>
        private class TransformationSource : Transformer<string, string, TransformationConfig>
        {
            protected override string DoTransform(string value)
            {
                return value;
            }
        }

        /// <summary>
        /// Sink transformation for the chain.
        /// </summary>
        private class TransformationSink : ITransformationSink<string>
        {
            public Type InputType
            {
                get
                {
                    return typeof(string);
                }
            }

            public string Value { get; private set; }

            public void Transform(string value)
            {
                this.Value = value;
            }
        }
    }
}