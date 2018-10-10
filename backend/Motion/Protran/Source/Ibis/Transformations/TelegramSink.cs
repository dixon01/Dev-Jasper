// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramSink.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TelegramSink type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Transformations
{
    using System;

    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Motion.Protran.Core.Transformations;

    /// <summary>
    /// Base class for all telegram sinks.
    /// </summary>
    public abstract class TelegramSink : ITransformationSink
    {
        /// <summary>
        /// Gets the input type of this sink.
        /// </summary>
        public abstract Type InputType { get; }

        /// <summary>
        /// Gets or sets the telegram to be filled when this sink
        /// receives data in <see cref="ITransformationSink{T}.Transform"/>.
        /// </summary>
        public Telegram Telegram { get; set; }
    }
}