// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfigurator.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Interface for classes that can be used to configure <see cref="MessageDispatcher" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Config
{
    /// <summary>
    /// Interface for classes that can be used to configure <see cref="MessageDispatcher"/>.
    /// </summary>
    public interface IConfigurator
    {
        /// <summary>
        /// Creates the local Medi address.
        /// </summary>
        /// <returns>
        /// The local Medi address.
        /// </returns>
        MediAddress CreateLocalAddress();

        /// <summary>
        /// Creates the <see cref="MediConfig"/> object to configure 
        /// <see cref=" MessageDispatcher"/>.
        /// </summary>
        /// <returns>
        /// The config object.
        /// </returns>
        MediConfig CreateConfig();
    }
}
