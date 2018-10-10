// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisConfigContextMock.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisConfigContextMock type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Tests.Mocks
{
    using Gorba.Common.Configuration.Protran.Ibis;
    using Gorba.Common.Protocols.Ximple.Generic;

    /// <summary>
    /// Mock for <see cref="IIbisConfigContext"/>.
    /// </summary>
    public class IbisConfigContextMock : IIbisConfigContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IbisConfigContextMock"/> class.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary.
        /// </param>
        /// <param name="config">
        /// The IBIS config (can be null).
        /// </param>
        public IbisConfigContextMock(Dictionary dictionary, IbisConfig config = null)
        {
            this.Dictionary = dictionary;
            this.Config = config;
        }

        /// <summary>
        /// Gets the entire IBIS config.
        /// </summary>
        public IbisConfig Config { get; private set; }

        /// <summary>
        /// Gets the generic view dictionary.
        /// </summary>
        public Dictionary Dictionary { get; private set; }

        /// <summary>
        /// Gets the absolute path related to the IBIS configuration file path.
        /// </summary>
        /// <param name="file">
        /// The absolute or related file path.
        /// </param>
        /// <returns>
        /// The absolute path to the given file.
        /// </returns>
        public string GetAbsolutePathRelatedToConfig(string file)
        {
            return file;
        }
    }
}