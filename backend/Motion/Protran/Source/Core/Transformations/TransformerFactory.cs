// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransformerFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Factory for Transformer implementations.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Core.Transformations
{
    using System;

    using Gorba.Common.Configuration.Protran.Transformations;

    /// <summary>
    /// Factory for Transformer implementations.
    /// </summary>
    public static class TransformerFactory
    {
        /// <summary>
        /// Creates and configures a transformer for the given configuration.
        /// </summary>
        /// <param name="config">
        /// the configuration for which the transformer is to be created.
        /// </param>
        /// <param name="inputType">
        /// The input type to be supported by the new transformer.
        /// </param>
        /// <returns>
        /// the transformer for this configuration.
        /// Its <see cref="ITransformer.Configure"/> method has already been called
        /// <exception cref="NullReferenceException">
        /// The type cannot be null.
        /// </exception>
        /// </returns>
        public static ITransformer CreateTransformer(TransformationConfig config, Type inputType)
        {
            // transformer implementation has to be in the same namespace as this class and have
            // the name of the config class plus "Transformer" appended.
            var name = string.Format("{0}.{1}Transformer", typeof(TransformerFactory).Namespace, config.GetType().Name);
            var type = Type.GetType(name, true, true);
            if (type == null)
            {
                throw new NullReferenceException("Type can not be null!");
            }

            var transformer = (ITransformer)Activator.CreateInstance(type);

            while (transformer.InputType != inputType && inputType.IsArray && inputType != typeof(byte[]))
            {
                // we have a transformer that expects a simple type,
                // but the previous transformation provided an array of
                // that type, let's wrap the transformer
                var wrapperType = typeof(ArrayTransformerWrapper<,,>).MakeGenericType(
                    transformer.InputType, transformer.OutputType, transformer.ConfigType);
                transformer = (ITransformer)wrapperType.GetConstructors()[0].Invoke(new object[] { transformer });
            }

            transformer.Configure(config);
            return transformer;
        }
    }
}
