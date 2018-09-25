// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramSource.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Transformations
{
    using Gorba.Common.Configuration.Protran.Transformations;
    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Motion.Protran.Core.Transformations;

    /// <summary>
    /// The first (hidden) element of a chain that extracts the bytes from
    /// the telegram object.
    /// </summary>
    public class TelegramSource : Transformer<Telegram, byte[], TransformationConfig>
    {
        /// <summary>
        /// Extracts the payload from the telegram.
        /// </summary>
        /// <param name="value">the telegram.</param>
        /// <returns>the payload of the given telegram.</returns>
        protected override byte[] DoTransform(Telegram value)
        {
            return value.Payload;
        }
    }
}
