// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConfigurationSerializer.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IConfigurationSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Qube.Configuration
{
    using System.IO;
    using System.Threading.Tasks;

    using Gorba.Common.Configuration.EPaper.MainUnit;

    /// <summary>
    /// Serializes configurations into the binary format requested by (iqube) units.
    /// </summary>
    public interface IConfigurationSerializer
    {
        /// <summary>
        /// Serializes the given configuration into the binary format understood by units.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        /// <returns>
        /// The <see cref="Stream"/> containing the binary serialized configuration. The <see cref="Stream.Position"/>
        /// is reset to 0;
        /// </returns>
        Task<Stream> SerializeAsync(MainUnitConfig configuration);
    }
}