// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Transformer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.Transformations
{
    using System;

    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// Base class for all standard transformers.
    /// </summary>
    /// <typeparam name="TInput">
    /// the type of the input object of this transformer.
    /// </typeparam>
    /// <typeparam name="TOutput">
    /// The type of the output object of this transformer.
    /// </typeparam>
    /// <typeparam name="TConfig">
    /// The config type (has to be a subclass of <see cref="TransformationConfig"/>)
    /// </typeparam>
    public abstract class Transformer<TInput, TOutput, TConfig> : ITransformer, ITransformationSink<TInput>
        where TConfig : TransformationConfig
    {
        /// <summary>
        /// Gets the sink of this transformation.
        /// </summary>
        public ITransformationSink<TOutput> Next { get; private set; }

        /// <summary>
        /// Gets or sets the sink of this transformation.
        /// This is the implementation of the non-generic
        /// <see cref="ITransformationSink"/> interface.
        /// </summary>
        ITransformationSink ITransformationSource.Next
        {
            get
            {
                return this.Next;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                this.Next = value as ITransformationSink<TOutput>;
                if (this.Next == null)
                {
                    throw new ArgumentException(
                        this.GetType().Name + " expects a " + typeof(ITransformationSink<TOutput>).Name
                        + " as next transformation.",
                        "value");
                }
            }
        }

        /// <summary>
        /// Gets the type of object that is expected by this source.
        /// This property always returns typeof(TInput).
        /// </summary>
        public Type InputType
        {
            get
            {
                return typeof(TInput);
            }
        }

        /// <summary>
        /// Gets the type of object that is output by this source.
        /// This property always returns typeof(TOutput).
        /// </summary>
        public Type OutputType
        {
            get
            {
                return typeof(TOutput);
            }
        }

        /// <summary>
        /// Gets the type of config object supported by this transformer.
        /// </summary>
        public Type ConfigType
        {
            get
            {
                return typeof(TConfig);
            }
        }

        /// <summary>
        /// Gets the configuration object of this transformer.
        /// </summary>
        TransformationConfig ITransformer.Config
        {
            get
            {
                return this.Config;
            }
        }

        /// <summary>
        /// Gets the configuration object of this transformer.
        /// </summary>
        protected TConfig Config { get; private set; }

        /// <summary>
        /// Transforms the given object by calling <see cref="DoTransform(TInput)"/>
        /// and then calling <see cref="Transform"/> on the next object in the chain.
        /// </summary>
        /// <param name="value">
        /// The value to be transformed.
        /// </param>
        public void Transform(TInput value)
        {
            this.Next.Transform(this.DoTransform(value));
        }

        /// <summary>
        /// Configures this transformer with the given configuration.
        /// </summary>
        /// <param name="config">the configuration.</param>
        void ITransformer.Configure(TransformationConfig config)
        {
            var configImpl = config as TConfig;
            if (configImpl == null)
            {
                throw new ArgumentException(this.GetType().Name + " expects a " +
                    typeof(TConfig).Name + " as configuration.");
            }

            this.Configure(configImpl);
        }

        /// <summary>
        /// Configures this transformer with the given configuration.
        /// </summary>
        /// <param name="config">
        /// The configuration.
        /// </param>
        protected virtual void Configure(TConfig config)
        {
            this.Config = config;
        }

        /// <summary>
        /// Actual transformation method to be implemented by subclasses.
        /// </summary>
        /// <param name="value">the value to be transformed.</param>
        /// <returns>the transformed value.</returns>
        protected abstract TOutput DoTransform(TInput value);
    }
}
