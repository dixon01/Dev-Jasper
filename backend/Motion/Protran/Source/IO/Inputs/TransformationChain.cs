// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransformationChain.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   A chain of transformers that transform a string.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.IO.Inputs
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Protran.Transformations;
    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Motion.Protran.Core.Transformations;

    /// <summary>
    /// A chain of transformers that transform a string.
    /// </summary>
    public class TransformationChain
    {
        private readonly TransformationSink sink = new TransformationSink();

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformationChain"/> class.
        /// </summary>
        /// <param name="transformations">
        /// The transformation configurations.
        /// </param>
        public TransformationChain(IEnumerable<TransformationConfig> transformations)
        {
            var first = new NameSource();
            this.First = first;
            ITransformer previous = first;
            foreach (var transcoderConfig in transformations)
            {
                var current = TransformerFactory.CreateTransformer(transcoderConfig, previous.OutputType);
                if (previous == first && current.InputType == typeof(int))
                {
                    // the first item in the chain expects an integer,
                    // so we provide the value instead of the name
                    var valueFirst = new ValueSource();
                    this.First = valueFirst;
                    previous = valueFirst;
                }

                previous.Next = current;
                previous = current;
            }

            previous.Next = this.sink;
        }

        /// <summary>
        /// Gets the first element in the chain.
        /// </summary>
        public ITransformationSink<IOValue> First { get; private set; }

        /// <summary>
        /// Transforms the value with this chain.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// the transformed value.
        /// </returns>
        public virtual string Transform(IOValue value)
        {
            this.First.Transform(value);
            return this.sink.Value;
        }

        /// <summary>
        /// Source transformer for the chain.
        /// </summary>
        private class NameSource : Transformer<IOValue, string, TransformationConfig>
        {
            protected override string DoTransform(IOValue value)
            {
                return value.Name;
            }
        }

        /// <summary>
        /// Source transformer for the chain.
        /// </summary>
        private class ValueSource : Transformer<IOValue, int, TransformationConfig>
        {
            protected override int DoTransform(IOValue value)
            {
                return value.Value;
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