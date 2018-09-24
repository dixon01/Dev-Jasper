// Gorba.Motion.Protran
// Luminator.Protran.MessagingProtocol
// $Rev::                                   
// 

namespace Luminator.Motion.Protran.AdHocMessagingProtocol.Interfaces
{
    using System;


    public interface IAdHocMessageServiceConfig
    {
        Uri AdHocApiUri { get; set; }

        Uri DestinationsApiUri { get; set; }

        int ApiTimeout { get; set; }

        int MaxAdHocMessages { get; set; }
    }
}