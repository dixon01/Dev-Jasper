// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringTelegramSink.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Transformations
{
    using System;

    using Gorba.Common.Protocols.Vdv300.Telegrams;
    using Gorba.Motion.Protran.Core.Transformations;

    /// <summary>
    /// Final sink in a transformation chain that puts the given
    /// string into the <see cref="StringTelegram.Data"/> property.
    /// </summary>
    public class StringTelegramSink : TelegramSink, ITransformationSink<string>
    {
        /// <summary>
        /// Gets the type of object that is expected by this source.
        /// This property always returns typeof(string).
        /// </summary>
        public override Type InputType
        {
            get
            {
                return typeof(string);
            }
        }

        /// <summary>
        /// Sets the <see cref="StringTelegram.Data"/> property
        /// of the telegram provided in the constructor to the given
        /// value.
        /// Despite its name, this method doesn't do any transformation.
        /// </summary>
        /// <param name="value">the value to be set.</param>
        public void Transform(string value)
        {
            ((StringTelegram)this.Telegram).Data = value;
        }
    }
}
