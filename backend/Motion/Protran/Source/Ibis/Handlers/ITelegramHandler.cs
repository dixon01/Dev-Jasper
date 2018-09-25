// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITelegramHandler.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ITelegramHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Handlers
{
    using Gorba.Common.Configuration.Protran.Ibis.Telegrams;

    /// <summary>
    /// Handler for telegrams of a certain type.
    /// </summary>
    public interface ITelegramHandler : IInputHandler
    {
        /// <summary>
        /// Gets a value indicating whether this handler is enabled.
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// Configures this handler with the given telegram config and
        /// generic view dictionary.
        /// </summary>
        /// <param name="config">
        /// The telegram config.
        /// </param>
        /// <param name="configContext">
        /// The configuration context.
        /// </param>
        void Configure(TelegramConfig config, IIbisConfigContext configContext);
    }
}
