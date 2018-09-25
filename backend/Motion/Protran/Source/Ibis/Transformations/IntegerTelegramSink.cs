// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegerTelegramSink.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IntegerTelegramSink type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Transformations
{
    using System;

    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Motion.Protran.Core.Transformations;

    /// <summary>
    /// Final sink in a transformation chain that puts the given
    /// int into the <see cref="IntegerTelegram.Data"/> property.
    /// </summary>
    public class IntegerTelegramSink : TelegramSink, ITransformationSink<int>
    {
        /// <summary>
        /// Gets the type of object that is expected by this source.
        /// This property always returns typeof(int).
        /// </summary>
        public override Type InputType
        {
            get
            {
                return typeof(int);
            }
        }

        /// <summary>
        /// Sets the <see cref="IntegerTelegram.Data"/> property
        /// of the telegram provided in the constructor to the given
        /// value.
        /// Despite its name, this method doesn't do any transformation.
        /// </summary>
        /// <param name="value">the value to be set.</param>
        public void Transform(int value)
        {
            ((IntegerTelegram)this.Telegram).Data = value;
        }
    }
}
