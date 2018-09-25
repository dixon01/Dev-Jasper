// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AnalogClockHandMode.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The mode of a hand of an analog clock.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Layout
{
    /// <summary>
    /// The mode of a hand of an analog clock.
    /// </summary>
    public enum AnalogClockHandMode
    {
        /// <summary>
        /// The hand moves smoothly around the clock face.
        /// </summary>
        Smooth,

        /// <summary>
        /// The hand jumps in entire unit values around the clock face.
        /// In this mode the minute and seconds hand jump 60 times, the
        /// hour hand 12 times for an entire circle.
        /// </summary>
        Jump,

        /// <summary>
        /// This mode is only supported for the seconds hand and simulates
        /// the Swiss railway clock by speeding up the round and then waiting
        /// 1500 milliseconds at the '0' position.
        /// </summary>
        /// <seealso cref="http://en.wikipedia.org/wiki/Swiss_railway_clock"/>
        CatchUp
    }
}