// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIbisConfigContext.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IIbisConfigContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis
{
    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Common.Protocols.Ximple.Generic;

    /// <summary>
    /// Interface to have access to the configuration.
    /// </summary>
    public interface IIbisConfigContext
    {
        /// <summary>
        /// Gets the entire IBIS config.
        /// </summary>
        IbisConfig Config { get; }

        /// <summary>
        /// Gets the generic view dictionary.
        /// </summary>
        Dictionary Dictionary { get; }

        /// <summary>
        /// Gets the absolute path related to the IBIS configuration file path.
        /// </summary>
        /// <param name="file">
        /// The absolute or related file path.
        /// </param>
        /// <returns>
        /// The absolute path to the given file.
        /// </returns>
        string GetAbsolutePathRelatedToConfig(string file);
    }
}