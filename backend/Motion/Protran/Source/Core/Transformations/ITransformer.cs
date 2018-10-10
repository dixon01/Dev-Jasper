// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITransformer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.Transformations
{
    using System;

    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// Interface for a transformer used in transformation chain.
    /// It combines a sink and a source and provides a method to
    /// configure the transformer.
    /// </summary>
    public interface ITransformer : ITransformationSink, ITransformationSource
    {
        /// <summary>
        /// Gets the type of config object supported by this transformer.
        /// </summary>
        Type ConfigType { get; }

        /// <summary>
        /// Gets the configuration object of this transformer.
        /// </summary>
        TransformationConfig Config { get; }

        /// <summary>
        /// Configures the transformer with the given configuration.
        /// In subclasses this will be a subclass of <see cref="TransformationConfig"/>.
        /// </summary>
        /// <param name="conifg">configuration object.</param>
        void Configure(TransformationConfig conifg);
    }
}
