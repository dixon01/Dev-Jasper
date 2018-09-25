namespace Luminator.Motion.Protran.AdHocMessagingProtocol.Interfaces
{
    using System;

    public interface ITimerSettings
    {
        TimeSpan Interval { get; set; }

        string IntervalXml { get; set; }

        bool IsValid { get; }
    }
}