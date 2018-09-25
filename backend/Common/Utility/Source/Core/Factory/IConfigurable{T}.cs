// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfigurable{T}.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IConfigurable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core.Factory
{
    /// <summary>
    /// Interface for a type that can be configured using a certain object.
    /// </summary>
    /// <typeparam name="T">
    /// the type of the configuration object to configure the object implementing this interface.
    /// </typeparam>
    public interface IConfigurable<T>
    {
        /// <summary>
        /// Configures this object.
        /// </summary>
        /// <param name="config">
        /// The config object.
        /// </param>
        void Configure(T config);
    }
}
