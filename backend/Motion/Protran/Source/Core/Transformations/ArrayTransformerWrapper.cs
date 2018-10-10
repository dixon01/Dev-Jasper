// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArrayTransformerWrapper.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ArrayTransformerWrapper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.Transformations
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// Wrapper around a transformer that allows for arrays to be converted
    /// item by item.
    /// </summary>
    /// <typeparam name="TInput">
    /// The type of the input array items.
    /// </typeparam>
    /// <typeparam name="TOutput">
    /// The type of the output array items.
    /// </typeparam>
    /// <typeparam name="TConfig">
    /// The type of configuration object used for the wrapped transformer.
    /// </typeparam>
    public class ArrayTransformerWrapper<TInput, TOutput, TConfig> : Transformer<TInput[], TOutput[], TConfig>
        where TConfig : TransformationConfig
    {
        private readonly Sink sink;
        private readonly Transformer<TInput, TOutput, TConfig> transformer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayTransformerWrapper{TInput,TOutput,TConfig}"/> class.
        /// </summary>
        /// <param name="transformer">
        /// The transformer that will be wrapped.
        /// </param>
        public ArrayTransformerWrapper(Transformer<TInput, TOutput, TConfig> transformer)
        {
            this.sink = new Sink();
            this.transformer = transformer;
            ((ITransformer)this.transformer).Next = this.sink;
        }

        /// <summary>
        /// Configures this transformer with the given configuration.
        /// </summary>
        /// <param name="config">
        /// The configuration.
        /// </param>
        protected override void Configure(TConfig config)
        {
            base.Configure(config);
            ((ITransformer)this.transformer).Configure(config);
        }

        /// <summary>
        /// Actual transformation method.
        /// </summary>
        /// <param name="value">the value to be transformed.</param>
        /// <returns>the transformed value.</returns>
        protected override TOutput[] DoTransform(TInput[] value)
        {
            foreach (var item in value)
            {
                this.transformer.Transform(item);
            }

            var result = this.sink.ToArray();
            this.sink.Clear();
            return result;
        }

        /// <summary>
        /// Inner class that provides the necessary transformation sink
        /// interface needed by the wrapped transformer.
        /// </summary>
        private class Sink : ITransformationSink<TOutput>
        {
            private readonly List<TOutput> items = new List<TOutput>();

            public Type InputType
            {
                get
                {
                    return typeof(TOutput);
                }
            }

            public TOutput[] ToArray()
            {
                return this.items.ToArray();
            }

            public void Clear()
            {
                this.items.Clear();
            }

            // COS:
            // If I enable the following way to implement
            // the method "Transform", the test project doesn't compile.
            // so, I added the other way below.
            public void Transform(TOutput value)
            {
                this.items.Add(value);
            }
        }
    }
}
