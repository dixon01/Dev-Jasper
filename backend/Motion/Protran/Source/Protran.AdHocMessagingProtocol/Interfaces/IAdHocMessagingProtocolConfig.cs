// InfomediaAll
// Luminator.Protran.AdHocMessagingProtocol
// <author>Kevin Hartman</author>
// $Rev::                                   
// 

namespace Luminator.Motion.Protran.AdHocMessagingProtocol.Interfaces
{
    using Luminator.Motion.Protran.AdHocMessagingProtocol.Models;

    public interface IAdHocMessagingProtocolConfig
    {
        UriSettings AdHocApiUri { get; set; }

        TimerSettings AdHocMessageTimerSettings { get; set; }

        int ApiTimeout { get; set; }

        UriSettings DestinationsApiUri { get; set; }

        bool EnableUnitRegistration { get; set; }

        int MaxAdHocMessages { get; set; }

        int MaxAdhocRegistrationAttempts { get; set; }

        TimerSettings RegisterUnitTimerSettings { get; set; }

        TimerSettings RequestUnitInfoTimerSettings { get; set; }
    }
}