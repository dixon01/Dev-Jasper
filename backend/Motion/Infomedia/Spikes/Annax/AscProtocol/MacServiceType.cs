namespace Gorba.Motion.Infomedia.Spikes.Annax.AscProtocol
{
    using System;

    [Flags]
    public enum MacServiceType
    {
        PollFrame = 0x40,
        SimpleFrame = 0x80,
        AnsweredFrame = 0xC0,
    }
}