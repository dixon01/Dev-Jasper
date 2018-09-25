// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransformationChain.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TransformationChain type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Vdv301.Handlers
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Protran.Transformations;
    using Gorba.Motion.Protran.Core.Transformations;

    /// <summary>
    /// The transformation chain that transforms content.
    /// </summary>
    /// <typeparam name="T">
    /// The input type of this chain.
    /// </typeparam>
    public class TransformationChain<T>
    {
        private readonly TransformationSink sink = new TransformationSink();

        /// <summary>
        /// Initializes a new instance of the <see cref="TransformationChain{T}"/> class.
        /// </summary>
        /// <param name="transfRef">
        /// The transformation reference.
        /// </param>
        /// <param name="context">
        /// The configuration context.
        /// </param>
        public TransformationChain(string transfRef, IHandlerConfigContext context)
        {
            this.Name = transfRef;

            var first = new TransformationSource();
            this.First = first;
            ITransformer previous = first;

            if (!string.IsNullOrEmpty(transfRef))
            {
                var config = context.Config.Transformations.Find(c => c.Id == transfRef);
                if (config == null)
                {
                    throw new KeyNotFoundException("Couldn't find transformation chain " + transfRef);
                }

                var transformations = config.ResolveReferences(context.Config.Transformations);

                foreach (var transcoderConfig in transformations)
                {
                    var current = TransformerFactory.CreateTransformer(transcoderConfig, previous.OutputType);
                    previous.Next = current;
                    previous = current;
                }
            }

            if (previous.OutputType == typeof(string[]) && this.sink.InputType == typeof(string))
            {
                // take the first element if the output is an array
                var current = new TakeFirstString();
                previous.Next = current;
                previous = current;
            }

            previous.Next = this.sink;
        }

        /// <summary>
        /// Gets the first element in the chain.
        /// </summary>
        public ITransformationSink<T> First { get; private set; }

        /// <summary>
        /// Gets the name of this chain.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Transforms the value with this chain.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// the transformed value.
        /// </returns>
        public virtual string Transform(T value)
        {
            this.First.Transform(value);
            return this.sink.Value;
        }

        private class TransformationSource : Transformer<T, T, TransformationConfig>
        {
            protected override T DoTransform(T value)
            {
                return value;
            }
        }

        private class TakeFirstString : Transformer<string[], string, TransformationConfig>
        {
            protected override string DoTransform(string[] value)
            {
                return value.Length == 0 ? string.Empty : value[0];
            }
        }

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