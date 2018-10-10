namespace Gorba.Motion.Infomedia.Spikes.Annax.AscProtocol.Commands
{
    using System;

    [Flags]
    public enum BaseAttr
    {
        NoAction,
        ScrollUp,
        ScrollDown,
        ScrollRight,
        ScrollLeft,
        FlipUp,
        FlipDown,
        FlipRight,
        FlipLeft,
        BlinkNormalBlank,
        BlinkNormalInvers,
        BlinkInversBlank,

        /// <summary>
        /// Scroll only if bitmap is larger than window
        /// </summary>
        AutoScroll = 16,

        /// <summary>
        /// Blink synchronously to master pulse.
        /// </summary>
        Synchronize = 32,

        /// <summary>
        /// Show the time of day.
        /// </summary>
        ClockFunction = 64,

        NotUsed = 0xFF,
    }
}